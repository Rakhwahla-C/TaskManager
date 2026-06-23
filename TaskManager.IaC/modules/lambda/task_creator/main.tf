resource "aws_lambda_function" "task_creator" {
  function_name = "task_creator"
  handler       = "TaskManager.TaskCreator::TaskManager.TaskCreator.TaskManager.Api.Function::FunctionHandler"
  runtime       = "dotnet8"
  role          = var.lambda_role_arn
  filename      = var.lambda_zip_path

  environment {
    variables = {
      TABLE_NAME = var.table_name
    }
  }
}
