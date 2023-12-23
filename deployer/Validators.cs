using Docker.DotNet;

namespace deployer.Validators
{
    public class ImageValidator
    {
        private DockerClient _dockerClient;

        public ImageValidator(DockerClient dockerClient)
        {
            _dockerClient = dockerClient;
        }

        public async Task<bool> ImageExists(string image)
        {
            try
            {
                var imageResult = await _dockerClient.Images.InspectImageAsync(image);
            }
            catch (DockerImageNotFoundException)
            {
                return false;
            }

            return true;
        }
    }

    public class ProjectValidator
    { }
}