using System.CommandLine;
using Pulumi;
using Pulumi.Automation;

namespace deployer
{
    class Program
    {
        static int Main(string[] args)
        {
            Option<string> imageOption = new(
                name: "--image",
                description: @"The fully qualified image name and tag to deploy
                E.g. 123456789012.dkr.ecr.eu-west-1.amazonaws.com/foobar:abcd123"
            );

            Option<DeployType> typeOption = new(
                name: "--type",
                description: "The type of project to deploy"
            );

            Option<string> projectDirectoryOption = new(
                name: "--project",
                description: "The directory of the infrastructure project to deploy from"
            );

            RootCommand rootCommand = new("Deploy ECS tasks over base infrastructure with Pulumi");
            rootCommand.AddOption(imageOption);
            rootCommand.AddOption(typeOption);
            rootCommand.AddOption(projectDirectoryOption);

            rootCommand.SetHandler(
                (image, type, projectDirectory) => { Handler(image, type, projectDirectory); },
                imageOption,
                typeOption,
                projectDirectoryOption
            );

            return rootCommand.Invoke(args);
        }

        static void Handler(string image, DeployType deployType, string projectDirectory)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
        }
    }
}