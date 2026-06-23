resource "aws_dynamodb_table" "task_manager_db" {
  name         = "task-manager"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "id"

  attribute {
    name = "id"
    type = "S"
  }

  tags = {
    Environment = "localstack"
  }
}