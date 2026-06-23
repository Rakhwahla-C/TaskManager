resource "aws_sns_topic" "task_events"{
    name = "TaskEvents-SNS"
}

resource "aws_sns_topic_subscription" "task_events_subscription" {
  topic_arn = aws_sns_topic.task_events.arn
  protocol  = "lambda"
  endpoint  = var.task_auditor_lambda_arn
}