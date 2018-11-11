namespace MsBuildResyncProjectReferenceGuid
{
    class Program
    {
        static void Main(string[] args)
        {
            string targetDirectory = @"S:\TimsSVN\8x\Trunk";
            ResyncProjectReferenceGuid.Execute(targetDirectory, true);
        }
    }
}
