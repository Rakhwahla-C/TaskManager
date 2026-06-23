output "invoke_url" {
  description = "Base URL of the deployed API (including stage)"
  value       = "http://localhost:4566/restapis/${aws_api_gateway_rest_api.task_api.id}/${aws_api_gateway_stage.dev.stage_name}/_user_request_"
}

output "tasks_url" {
  description = "Direct URL to the /tasks endpoint"
  value       = "http://localhost:4566/restapis/${aws_api_gateway_rest_api.task_api.id}/${aws_api_gateway_stage.dev.stage_name}/_user_request_/tasks"
}