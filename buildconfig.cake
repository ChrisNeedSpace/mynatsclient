public class BuildConfig
{
    private const string Version = "0.3.0";

    public readonly string SrcDir = "./src/";
    public readonly string OutDir = "./build/";    
    
    public string Target { get; private set; }
    public string Branch { get; private set; }
    public string SemVer { get; private set; }
    public string BuildProfile { get; private set; }
    public bool IsTeamCityBuild { get; private set; }
    
    public static BuildConfig Create(
        ICakeContext context,
        BuildSystem buildSystem)
    {
        if (context == null)
            throw new ArgumentNullException("context");

        var target = context.Argument("target", "Default");
        var branch = context.Argument("branch", string.Empty);
        var branchIsRelease = branch.ToLower() == "release";
        var buildRevision = context.Argument("buildrevision", "0");

        return new BuildConfig
        {
            Target = target,
            Branch = branch,
            SemVer = Version + (branchIsRelease ? string.Empty : "-b" + buildRevision),
            BuildProfile = context.Argument("configuration", "Release"),
            IsTeamCityBuild = buildSystem.TeamCity.IsRunningOnTeamCity
        };
    }
}