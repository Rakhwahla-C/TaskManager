

resource "aws_api_gateway_rest_api" "task_api" {
  name        = var.api_name
  description = "Task Manager API"

  endpoint_configuration {
    types = ["REGIONAL"]
  }
}


resource "aws_api_gateway_resource" "tasks" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  parent_id   = aws_api_gateway_rest_api.task_api.root_resource_id
  path_part   = "tasks"
}


resource "aws_api_gateway_method" "get_tasks" {
  rest_api_id   = aws_api_gateway_rest_api.task_api.id
  resource_id   = aws_api_gateway_resource.tasks.id
  http_method   = "GET"
  authorization = "NONE"
}

resource "aws_api_gateway_method" "post_tasks" {
  rest_api_id   = aws_api_gateway_rest_api.task_api.id
  resource_id   = aws_api_gateway_resource.tasks.id
  http_method   = "POST"
  authorization = "NONE"
}

# CORS Preflight Options
resource "aws_api_gateway_method" "options" {
  rest_api_id   = aws_api_gateway_rest_api.task_api.id
  resource_id   = aws_api_gateway_resource.tasks.id
  http_method   = "OPTIONS"
  authorization = "NONE"
}


# LAMBDA permission

resource "aws_lambda_permission" "allow_apigw" {
  statement_id  = "AllowAPIGatewayInvoke"
  action        = "lambda:InvokeFunction"
  function_name = var.lambda_function_name
  principal     = "apigateway.amazonaws.com"


  source_arn = "${aws_api_gateway_rest_api.task_api.execution_arn}/*/*"
}

#Proxy to lambda

resource "aws_api_gateway_integration" "get_tasks_integration" {
  rest_api_id             = aws_api_gateway_rest_api.task_api.id
  resource_id             = aws_api_gateway_resource.tasks.id
  http_method             = aws_api_gateway_method.get_tasks.http_method
  integration_http_method = "POST"        
  type                    = "AWS_PROXY"
  uri                     = var.lambda_invoke_arn
}

resource "aws_api_gateway_integration" "post_tasks_integration" {
  rest_api_id             = aws_api_gateway_rest_api.task_api.id
  resource_id             = aws_api_gateway_resource.tasks.id
  http_method             = aws_api_gateway_method.post_tasks.http_method
  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = var.lambda_invoke_arn
}

resource "aws_api_gateway_integration" "options_integration" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  resource_id = aws_api_gateway_resource.tasks.id
  http_method = aws_api_gateway_method.options.http_method
  type        = "MOCK"

  request_templates = {
    "application/json" = "{\"statusCode\": 200}"
  }
}


# CORS


resource "aws_api_gateway_method_response" "options_200" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  resource_id = aws_api_gateway_resource.tasks.id
  http_method = aws_api_gateway_method.options.http_method
  status_code = "200"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Headers" = true
    "method.response.header.Access-Control-Allow-Methods" = true
    "method.response.header.Access-Control-Allow-Origin"  = true
  }

  response_models = {
    "application/json" = "Empty"
  }
}

resource "aws_api_gateway_integration_response" "options_integration_response" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  resource_id = aws_api_gateway_resource.tasks.id
  http_method = aws_api_gateway_method.options.http_method
  status_code = aws_api_gateway_method_response.options_200.status_code

  response_parameters = {
    "method.response.header.Access-Control-Allow-Headers" = "'Content-Type,Authorization,X-Amz-Date,X-Api-Key'"
    "method.response.header.Access-Control-Allow-Methods" = "'GET,POST,DELETE,PUT,OPTIONS'"
    "method.response.header.Access-Control-Allow-Origin"  = "'http://localhost:3000'"  
  }
  depends_on = [
    aws_api_gateway_integration.options_integration
  ]
}

resource "aws_api_gateway_method_response" "get_200" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  resource_id = aws_api_gateway_resource.tasks.id
  http_method = aws_api_gateway_method.get_tasks.http_method
  status_code = "200"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Origin" = true
  }
}

resource "aws_api_gateway_integration_response" "get_integration_response" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  resource_id = aws_api_gateway_resource.tasks.id
  http_method = aws_api_gateway_method.get_tasks.http_method
  status_code = aws_api_gateway_method_response.get_200.status_code

  response_parameters = {
    "method.response.header.Access-Control-Allow-Origin" = "'http://localhost:3000'"
  }

  depends_on = [aws_api_gateway_integration.get_tasks_integration]
}


resource "aws_api_gateway_method_response" "post_200" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  resource_id = aws_api_gateway_resource.tasks.id
  http_method = aws_api_gateway_method.post_tasks.http_method
  status_code = "200"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Origin" = true
  }
}

resource "aws_api_gateway_integration_response" "post_integration_response" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  resource_id = aws_api_gateway_resource.tasks.id
  http_method = aws_api_gateway_method.post_tasks.http_method
  status_code = aws_api_gateway_method_response.post_200.status_code

  response_parameters = {
    "method.response.header.Access-Control-Allow-Origin" = "'http://localhost:3000'"
  }

  depends_on = [aws_api_gateway_integration.post_tasks_integration]
}

resource "aws_api_gateway_resource" "task_id" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  parent_id   = aws_api_gateway_resource.tasks.id
  path_part   = "{id}"
}

resource "aws_api_gateway_method" "delete_task" {
  rest_api_id   = aws_api_gateway_rest_api.task_api.id
  resource_id   = aws_api_gateway_resource.task_id.id
  http_method   = "DELETE"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "delete_task_integration" {
  rest_api_id             = aws_api_gateway_rest_api.task_api.id
  resource_id             = aws_api_gateway_resource.task_id.id
  http_method             = aws_api_gateway_method.delete_task.http_method
  integration_http_method = "POST"       
  type                    = "AWS_PROXY"
  uri                     = var.lambda_invoke_arn
}

resource "aws_api_gateway_method_response" "delete_200" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  resource_id = aws_api_gateway_resource.task_id.id
  http_method = aws_api_gateway_method.delete_task.http_method
  status_code = "200"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Origin" = true
  }
}

resource "aws_api_gateway_integration_response" "delete_integration_response" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  resource_id = aws_api_gateway_resource.task_id.id
  http_method = aws_api_gateway_method.delete_task.http_method
  status_code = aws_api_gateway_method_response.delete_200.status_code

  response_parameters = {
    "method.response.header.Access-Control-Allow-Origin" = "'http://localhost:3000'"
  }

  depends_on = [aws_api_gateway_integration.delete_task_integration]
}



resource "aws_api_gateway_method" "put_task" {
  rest_api_id   = aws_api_gateway_rest_api.task_api.id
  resource_id   = aws_api_gateway_resource.task_id.id
  http_method   = "PUT"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "put_task_integration" {
  rest_api_id             = aws_api_gateway_rest_api.task_api.id
  resource_id             = aws_api_gateway_resource.task_id.id
  http_method             = aws_api_gateway_method.put_task.http_method
  integration_http_method = "POST"       
  type                    = "AWS_PROXY"
  uri                     = var.lambda_invoke_arn
}

resource "aws_api_gateway_method_response" "put_200" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  resource_id = aws_api_gateway_resource.task_id.id
  http_method = aws_api_gateway_method.put_task.http_method
  status_code = "200"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Origin" = true
  }
}

resource "aws_api_gateway_integration_response" "put_integration_response" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  resource_id = aws_api_gateway_resource.task_id.id
  http_method = aws_api_gateway_method.put_task.http_method
  status_code = aws_api_gateway_method_response.put_200.status_code

  response_parameters = {
    "method.response.header.Access-Control-Allow-Origin" = "'http://localhost:3000'"
  }

  depends_on = [aws_api_gateway_integration.put_task_integration]
}




resource "aws_api_gateway_method" "options_task_id" {
  rest_api_id   = aws_api_gateway_rest_api.task_api.id
  resource_id   = aws_api_gateway_resource.task_id.id
  http_method   = "OPTIONS"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "options_task_id_integration" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  resource_id = aws_api_gateway_resource.task_id.id
  http_method = aws_api_gateway_method.options_task_id.http_method
  type        = "MOCK"

  request_templates = {
    "application/json" = "{\"statusCode\": 200}"
  }
}

resource "aws_api_gateway_method_response" "options_task_id_200" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  resource_id = aws_api_gateway_resource.task_id.id
  http_method = aws_api_gateway_method.options_task_id.http_method
  status_code = "200"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Headers" = true
    "method.response.header.Access-Control-Allow-Methods" = true
    "method.response.header.Access-Control-Allow-Origin"  = true
  }

  response_models = {
    "application/json" = "Empty"
  }
}

resource "aws_api_gateway_integration_response" "options_task_id_integration_response" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id
  resource_id = aws_api_gateway_resource.task_id.id
  http_method = aws_api_gateway_method.options_task_id.http_method
  status_code = aws_api_gateway_method_response.options_task_id_200.status_code

  response_parameters = {
    "method.response.header.Access-Control-Allow-Headers" = "'Content-Type,Authorization,X-Amz-Date,X-Api-Key'"
    "method.response.header.Access-Control-Allow-Methods" = "'GET,POST,DELETE,PUT,OPTIONS'"
    "method.response.header.Access-Control-Allow-Origin"  = "'http://localhost:3000'"
  }
  depends_on = [
    aws_api_gateway_integration.options_task_id_integration
  ]
}




resource "aws_api_gateway_deployment" "dev" {
  rest_api_id = aws_api_gateway_rest_api.task_api.id

  depends_on = [
    aws_api_gateway_integration.get_tasks_integration,
    aws_api_gateway_integration.post_tasks_integration,
    aws_api_gateway_integration.options_integration,

    aws_api_gateway_integration_response.get_integration_response,
    aws_api_gateway_integration_response.post_integration_response,
    aws_api_gateway_integration_response.options_integration_response,

   
    aws_api_gateway_method.options_task_id,
    aws_api_gateway_integration.delete_task_integration,
    aws_api_gateway_integration.put_task_integration,
    aws_api_gateway_integration.options_task_id_integration,
    aws_api_gateway_resource.task_id,

    aws_api_gateway_integration_response.delete_integration_response,
    aws_api_gateway_integration_response.put_integration_response,
    aws_api_gateway_integration_response.options_task_id_integration_response
  ]

  triggers = {
    redeployment = sha1(jsonencode([
      aws_api_gateway_integration.get_tasks_integration.id,
      aws_api_gateway_integration.post_tasks_integration.id,
      aws_api_gateway_integration.options_integration.id,

      aws_api_gateway_integration_response.get_integration_response.id,
      aws_api_gateway_integration_response.post_integration_response.id,
      aws_api_gateway_integration_response.options_integration_response.id,

     
      aws_api_gateway_integration.delete_task_integration.id,
      aws_api_gateway_integration.put_task_integration.id,
      aws_api_gateway_integration.options_task_id_integration.id,

      aws_api_gateway_integration_response.delete_integration_response.id,
      aws_api_gateway_integration_response.put_integration_response.id,
      aws_api_gateway_integration_response.options_task_id_integration_response.id,

      aws_api_gateway_method.options_task_id.id,
      aws_api_gateway_resource.task_id.id,
    ]))
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_api_gateway_stage" "dev" {
  rest_api_id   = aws_api_gateway_rest_api.task_api.id
  deployment_id = aws_api_gateway_deployment.dev.id
  stage_name    = var.stage_name


  access_log_settings {
    destination_arn = "arn:aws:logs:us-east-1:000000000000:log-group:API-Gateway-Execution-Logs_${aws_api_gateway_rest_api.task_api.id}/${var.stage_name}"
    format          = "$context.requestId $context.status $context.responseLength"
  }
}





