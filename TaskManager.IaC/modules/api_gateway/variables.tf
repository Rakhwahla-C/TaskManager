variable "api_name" {
  type = string
}

variable "stage_name" {
  type = string
}

variable "lambda_function_name" {
  type = string
}

variable "lambda_invoke_arn" {
  type        = string
  description = "Lambda invoke ARN"
}

variable "region" {
  type    = string
  default = "us-east-1"
}
