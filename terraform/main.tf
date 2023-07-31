provider "aws" {
  region = var.aws_region
}

# store the tf state in a secure aws bucket, uncomment the following lines for development purposes 
# terraform {
#   backend "s3" {
#     bucket = "your-bucket-name"   # Replace with your actual S3 bucket name
#     key    = "your-state-file.tfstate"  # Replace with your desired state file name
#     region = "your-aws-region"    # Replace with your AWS region where the S3 bucket is located
#   }
#}

resource "aws_appconfig_application" "appconfig_application" {
  name        = var.app_name
  description = var.app_description
}

resource "aws_appconfig_configuration_profile" "appconfig_profile" {
  name            = var.profile_name
  application_id  = aws_appconfig_application.appconfig_application.id
  description     = var.profile_description
  location_uri    = var.profile_location_uri
}

resource "aws_appconfig_hosted_configuration_version" "configuration" {
  application_id    = aws_appconfig_application.appconfig_application.id
  content = jsonencode({
    "CipherKey" = var.CipherKey,
    "CipherIv"  = var.CipherIv,
    "ApiKey"    = var.ApiKey
  })

  content_type = "application/json"
  configuration_profile_id = aws_appconfig_configuration_profile.appconfig_profile.id
}

resource "aws_appconfig_environment" "appconfig_environment" {
  name                = var.env_name
  description         = "Example AppConfig Environment"
  application_id      = aws_appconfig_application.appconfig_application.id
}

resource "aws_resourcegroups_group" "resourcegroup" {
  name = var.resource_group_name

  resource_query {
    query = <<JSON
{
  "ResourceTypeFilters": [
    "AWS::EC2::Instance"
  ],
  "TagFilters": [
    {
      "Key": "Stage",
      "Values": ["Test"]
    }
  ]
}
JSON
  }
}


resource "aws_iam_role" "lambda_role" {
  name = "lambda-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "lambda.amazonaws.com"
        }
      }
    ]
  })
}

resource "aws_iam_policy_attachment" "lambda_policy_attachment" {
  name       = "lambda-policy-attachment"
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
  roles      = [aws_iam_role.lambda_role.name]
}

resource "aws_lambda_function" "example_lambda" {
  function_name    = var.lambda_function_name
  handler          = "AuthenticationService::AuthenticationFunction::AuthenticateAsync"
  runtime          = "dotnet6"
  role             = aws_iam_role.lambda_role.arn
  source_code_hash = filebase64sha256(var.lambda_function_zip_path)
  filename         = var.lambda_function_zip_path                                                                                                                                                        
  publish          = true
}

resource "aws_iam_role_policy_attachment" "lambda_policy_attachment" {
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
  role       = aws_iam_role.lambda_role.name
}


resource "aws_dynamodb_table" "table" {
  name           = "Credentials"
  billing_mode   = "PAY_PER_REQUEST"
  hash_key       = "Id"
  attribute {
    name = "Id"
    type = "N"
  }
}

resource "aws_api_gateway_rest_api" "api_gateway" {
  name        = "Authentication API"
  description = "Authentication API Gateway"
}

resource "aws_api_gateway_resource" "api_gateway_resource" {
  rest_api_id = aws_api_gateway_rest_api.api_gateway.id
  parent_id   = aws_api_gateway_rest_api.api_gateway.root_resource_id
  path_part   = "resource"
}

resource "aws_api_gateway_method" "api_method" {
  rest_api_id   = aws_api_gateway_rest_api.api_gateway.id
  resource_id   = aws_api_gateway_resource.api_gateway_resource.id
  http_method   = "GET"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "api_integration" {
  rest_api_id = aws_api_gateway_rest_api.api_gateway.id
  resource_id = aws_api_gateway_resource.api_gateway_resource.id
  http_method = aws_api_gateway_method.api_method.http_method
  type        = "MOCK"
}

resource "aws_api_gateway_integration_response" "api_integration_response" {
  rest_api_id = aws_api_gateway_rest_api.api_gateway.id
  resource_id = aws_api_gateway_resource.api_gateway_resource.id
  http_method = aws_api_gateway_method.api_method.http_method
  status_code = "200"
  response_templates = {
    "application/json" = jsonencode({
      message = "Hello, World!"
    })
  }
}

resource "aws_api_gateway_method_response" "api_response_ok" {
  rest_api_id = aws_api_gateway_rest_api.api_gateway.id
  resource_id = aws_api_gateway_resource.api_gateway_resource.id
  http_method = aws_api_gateway_method.api_method.http_method
  status_code = "200"
  response_models = {
    "application/json" = "Empty"
  }
}

resource "aws_api_gateway_method_response" "api_response_fail" {
  rest_api_id = aws_api_gateway_rest_api.api_gateway.id
  resource_id = aws_api_gateway_resource.api_gateway_resource.id
  http_method = aws_api_gateway_method.api_method.http_method
  status_code = "401"
  response_models = {
    "application/json" = "Empty"
  }
}

resource "aws_api_gateway_deployment" "api_deployment" {
  rest_api_id = aws_api_gateway_rest_api.api_gateway.id
  stage_name  = var.env_name
}