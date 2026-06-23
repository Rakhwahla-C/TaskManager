variable "task_auditor_lambda_name" {
  type        = string
  description = "The name of the TaskAuditor Lambda function"
}

variable "sns_task_events_arn" {
  type        = string
  description = "The ARN of the TaskEvents SNS topic"
}