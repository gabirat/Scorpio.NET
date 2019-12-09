namespace Scorpio.ProcessRunner
{
    public interface IGenericProcessRunner
    {
        /// <summary>
        /// Runs command and return stdout.
        /// </summary>
        /// <param name="command">Command to run</param>
        /// <returns>stdout</returns>
        string RunCommand(string command);
    }
}
