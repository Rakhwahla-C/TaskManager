provider "aws" {
  region                      = "us-east-1"
  access_key                  = "test"
  secret_key                  = "test"
  skip_credentials_validation = true
  skip_requesting_account_id  = true
  endpoints {
    lambda      = "http://localhost:4566"
    dynamodb    = "http://localhost:4566"
    apigateway  = "http://localhost:4566"
    iam         = "http://localhost:4566"
    sns         = "http://localhost:4566"
  }
}