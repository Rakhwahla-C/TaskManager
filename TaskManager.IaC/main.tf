


module "dynamo" {
  source = "./modules/dynamo"
}


module "lambda_task_creator" {
  source          = "./modules/lambda/task_creator"
  lambda_role_arn = module.iam.lambda_role_arn
  lambda_zip_path = "${path.module}/lambda.zip" 
  table_name      = module.dynamo.table_name
}

module "lambda_task_auditor" {
  source          = "./modules/lambda/task_auditor"
  lambda_role_arn = module.iam.lambda_role_arn
  lambda_zip_path = "${path.module}/TaskAuditorZip.zip" 
}

module "sns" {
  source = "./modules/sns"
  task_auditor_lambda_arn  = module.lambda_task_auditor.lambda_arn
}
module "iam" {
  source = "./modules/iam"
  task_auditor_lambda_name = module.lambda_task_creator.lambda_function_name
  sns_task_events_arn      = module.sns.task_events_arn
}

module "api_gateway" {
  source               = "./modules/api_gateway"
  api_name             = "task-manager-api"
  stage_name           = "dev"
  lambda_function_name = module.lambda_task_creator.lambda_function_name
  lambda_invoke_arn    = module.lambda_task_creator.lambda_invoke_arn

  depends_on = [module.lambda_task_creator]
}


output "api_url" {
  description = "Base URL for API Gateway "
  value       = "${module.api_gateway.invoke_url}/"
}

output "tasks_endpoint" {
  description = "Direct /tasks endpoint"
  value       = "${module.api_gateway.invoke_url}/tasks"
}

output "test_commands" {
  value = <<-EOT

  API IS READY!

  GET all tasks:
  curl '${module.api_gateway.invoke_url}/tasks'


  EOT
}