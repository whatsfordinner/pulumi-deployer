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

    var jobPolicy = new Pulumi.Aws.Iam.Policy("jobPolicy", new()
    {
        Path = "/jobdoer/",
        NamePrefix = "jobExecutionPolicy",
        PolicyDocument = Output.Tuple(bucket.Arn, queue.Arn)
        .Apply(t =>
        {
            string bucketArn = t.Item1;
            string queueArn = t.Item2;

            var policy = new Dictionary<string, object?>
            {
                ["Version"] = "2012-10-17",
                ["Statement"] = new[]
                {
                    new Dictionary<string, object?>
                    {
                        ["Action"] = new[]
                        {
                            "s3:Get*",
                            "s3:Put*",
                            "s3:DeleteObject",
                        },
                        ["Effect"] = "Allow",
                        ["Resource"] = bucketArn,
                    },
                    new Dictionary<string, object?>
                    {
                        ["Action"] = new[]
                        {
                            "sqs:ReceiveMessage",
                            "sqs:DeleteMessage",
                        },
                        ["Effect"] = "Allow",
                        ["Resource"] = queueArn,
                    },
                }
            };

            return JsonSerializer.Serialize(policy);
        }),
    });

    var jobPolicyAttachment = new Pulumi.Aws.Iam.RolePolicyAttachment("jobPolicyAttachment", new()
    {
        PolicyArn = jobPolicy.Arn,
        Role = jobRole.Name,
    });

    // Export outputs here
    return new Dictionary<string, object?>
    {
        ["jobRole"] = jobRole.Name,
        ["environment"] = new Dictionary<string, object?>
        {
            ["JOB_QUEUE_URL"] = queue.Url,
            ["JOB_BUCKET_NAME"] = bucket.Id,
        },
        ["secrets"] = new Dictionary<string, object?>
        { },
    };
});
