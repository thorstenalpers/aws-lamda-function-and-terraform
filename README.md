
# Terraform
* Make sure that terraform is installed in the version 1.5.4
* Configure the deployment via terraform.tfvars or by cmd line parameters
* The tf state will be stored in a AWS Bucket, it must be created before running scripts. Uncomment those lines for development purposes.
* I have not tested it, because I have no AWS account

## Following resources are included:
* Amazon AppConfig
* Resource Group to isolate and group the resources
* DynamoDB Table
* Lambda Function
* API Gateway

# AuthenticationService
* I assume that the credentials sent via HTTP headers are already encrypted and therefore stored in the database in encrypted form.
* You can run the UnitTests, debugging was not possible for me anymore, because I have no AppConfig
* The Amazon AppConfig stores all configuration values, developers can also use environment variables
* UnitTests should be more, 90-100% code coverage is necessary, because it is a security related service
* The authentication and authorization mechanism to protect the lambda Function should be reviewed and adjusted
* IntegrationTests with e.g. Postman should be implemented
* The OpenAPI spec for the Lambda Function is not shown in Swagger.
* I have not tested it manually, because I have no AWS account
