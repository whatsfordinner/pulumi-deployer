using System.Collections.Generic;
using System.Text.Json;
using Pulumi;
using Pulumi.Aws;

return await Deployment.RunAsync(() =>
{
    // Add your resources here
    // e.g. var resource = new Resource("name", new ResourceArgs { });
    var queue = new Pulumi.Aws.Sqs.Queue("jobQueue", new()
    {
        NamePrefix = "job-queue",
    });

    var bucket = new Pulumi.Aws.S3.Bucket("artefactBucket", new()
    {
        BucketPrefix = "job-bucket",
    });

    var jobRole = new Pulumi.Aws.Iam.Role("jobRole", new()
    {
        Path = "/jobdoer/",
        NamePrefix = "jobExecutionRole",
        AssumeRolePolicy = JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["Version"] = "2012-10-17",
            ["Statement"] = new[]
            {
                new Dictionary<string, object?>
                {
                    ["Action"] = "sts:AssumeRole",
                    ["Effect"] = "Allow",
                    ["Principal"] = new Dictionary<string, object?>
                    {
                        ["Service"] = "ecs-tasks.amazonaws.com",
                    },
                }
            }
        }),
    });

    // Export outputs here
    return new Dictionary<string, object?>
    {
        ["outputKey"] = "outputValue"
    };
});
