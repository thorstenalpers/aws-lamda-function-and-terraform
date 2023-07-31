variable "aws_region" {
  description = "AWS Region for creating resources."
}

variable "app_name" {
  description = "Name of the AppConfig application."
}

variable "app_description" {
  description = "Description of the AppConfig application."
}

variable "profile_name" {
  description = "Name of the AppConfig configuration profile."
}

variable "profile_description" {
  description = "Description of the AppConfig configuration profile."
}

variable "profile_location_uri" {
  description = "URI of the AppConfig configuration source."
}

variable "env_name" {
  description = "Name of the AppConfig environment."
}

variable "env_description" {
  description = "Description of the AppConfig environment."
}

variable "resource_group_name" {
  description = "Name of the AWS Resource Group."
}

variable "CipherKey" {
  description = "The cipher key value for encryption."
  type        = string
}

variable "CipherIv" {
  description = "The cipher IV value for encryption."
  type        = string
}

variable "ApiKey" {
  description = "The API key to access the Authentication Service."
  type        = string
}

variable "lambda_function_name" {
  description = "Name of the Lambda function."
  type        = string
  default     = "example-lambda-function"
}

variable "lambda_function_zip_path" {
  description = "File path to the zip file containing the Lambda function code."
  type        = string
}