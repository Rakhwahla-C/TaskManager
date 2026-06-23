resource "aws_lambda_function" "task_auditor" {
  function_name = "task_auditor"
  handler       = "TaskManager.TaskAuditor::TaskManager.TaskAuditor.Function::FunctionHandler"
  runtime       = "dotnet8"
  role          = var.lambda_role_arn
  filename      = var.lambda_zip_path
}