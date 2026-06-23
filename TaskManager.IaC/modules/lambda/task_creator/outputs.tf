output "lambda_function_name" {
  value = aws_lambda_function.task_creator.function_name
}

output "lambda_arn" {
  value = aws_lambda_function.task_creator.arn
}

output "lambda_invoke_arn" {
  value = aws_lambda_function.task_creator.invoke_arn  
}