using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    [PathExecutable(name: "docker-compose")]
    public readonly Tool DockerCompose;

    [PathExecutable(name: "samlocal")]
    public readonly Tool SamLocal;

    public static int Main () => Execute<Build>(x => x.SamLocalBuild);

    [Parameter("Connection string to run the migration")]
    public string ConnectionString;
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestDirectory => RootDirectory / "test";
    AbsolutePath PublishDirectory => RootDirectory / "publish";

    string MigratorProject = "MyECommerceApp.Migrator";

    string TestProject = "MyECommerceApp.Tests";

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    public bool IsRunning()
    {
        var output = DockerCompose("ps --status running --quiet");
        return output.Count > 0;
    }

    Target StartEnv => _ => _
    .OnlyWhenDynamic(() => !IsRunning())
        .Executes(() =>
        {
            DockerCompose("up -d");
        });

    Target StopEnv => _ => _
        .Executes(() =>
        {
            DockerCompose("down");
        });

    Target SamLocalBuild => _ => _
        .Executes(() =>
        {
            SamLocal("build --template-file ./src/MyECommerceApp/template.yml");
        });

    Target SamLocalDeploy => _ => _
        .DependsOn(SamLocalBuild)
        .DependsOn(RunLocalMigrator)
        .Executes(() =>
        {
            SamLocal($"deploy --no-confirm-changeset --disable-rollback --resolve-s3 --s3-prefix myecommerceapp --stack-name myecommerceapp --region us-east-1  --capabilities CAPABILITY_IAM");
        });

    Target CleanMigrator => _ => _
        .Executes(() =>
        {
            EnsureCleanDirectory(PublishDirectory / MigratorProject);
        });

    Target CompileMigrator => _ => _
        .DependsOn(CleanMigrator)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(SourceDirectory / MigratorProject)
                .SetConfiguration(Configuration));
        });

    Target PublishMigrator => _ => _
        .DependsOn(CompileMigrator)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(SourceDirectory / MigratorProject)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetOutput(PublishDirectory / MigratorProject));
        });

    Target RunLocalMigrator => _ => _
        .DependsOn(StartEnv)
        .DependsOn(PublishMigrator)
        .Description("Apply the scripts over the database")
        .Executes(() =>
        {
            DotNet(PublishDirectory / MigratorProject / $"{MigratorProject}.dll");
        });

    Target Test => _ => _
        .Description("Run integration tests")
        .DependsOn(SamLocalDeploy)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(TestDirectory / TestProject)
                .SetConfiguration(Configuration));
        });

}
