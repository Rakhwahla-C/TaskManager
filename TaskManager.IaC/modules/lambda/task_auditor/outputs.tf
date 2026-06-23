output "lambda_function_name" {
  value = aws_lambda_function.task_auditor.function_name
}

output "lambda_arn" {
  value = aws_lambda_function.task_auditor.arn
}

output "lambda_invoke_arn" {
  value = aws_lambda_function.task_auditor.invoke_arn  
}