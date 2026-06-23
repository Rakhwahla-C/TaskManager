output "table_arn" {
  value = aws_dynamodb_table.task_manager_db.arn
}

output "table_name" {
  value = aws_dynamodb_table.task_manager_db.name
}