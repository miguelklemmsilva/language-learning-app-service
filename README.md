# Language Learning App Service

Welcome to the **Language Learning App** repository! This document provides a high-level overview of the project structure, the design decisions made, the infrastructure it relies on, and how to implement tests due to its modular architecture. It also highlights the benefits of Native AOT and how we utilize the AWS Lambda Annotations framework for clean, organized Lambda functions.

## Table of Contents

1. [Overview](#overview)
2. [Project Structure](#project-structure)
    1. [Core Layer](#core-layer)
    2. [Infrastructure Layer](#infrastructure-layer)
    3. [Lambda Layer](#lambda-layer)
3. [Design Decisions](#design-decisions)
4. [Infrastructure Details](#infrastructure-details)
5. [Flow of Execution](#flow-of-execution)
6. [Native AOT & AWS Lambda Annotations](#native-aot--aws-lambda-annotations)
7. [Testing Strategy](#testing-strategy)
8. [Why This Design Is Robust](#why-this-design-is-robust)
9. [Pipeline & CI/CD](#pipeline--cicd)

---

## Overview

This project is a serverless application designed to help users learn languages. It leverages:

- **AWS Lambda** (for compute),
- **DynamoDB** (for data storage),
- **Cognitive Services** (e.g., Azure Translation, OpenAI's ChatGPT) for advanced AI-based language processing,
- **.NET 8** with **Native AOT** compilation to improve cold-start performance,
- **AWS Lambda Annotations** for a streamlined approach to writing Lambda function handlers.

Within the code, you will find:
- A clear separation of concerns between domain logic, data access, and Lambda entry points.
- A strategy that makes the application easy to test, easy to maintain, and straightforward to deploy.

---

## Project Structure

The repository is logically split into **three main** projects/folders: **Core**, **Infrastructure**, and **Lambda**.

### Core Layer

**Directory:** `Core`

- **Purpose:** Holds the **domain logic** and **service classes**. It does not depend on any external frameworks or services directly. Instead, it uses interfaces that abstract away external dependencies (e.g., data stores, external APIs).
- **Key Folders/Files:**
    - **Helpers**: Utility classes (e.g., `AuthHelper`, `ResponseHelper`) that encapsulate repetitive or cross-cutting concerns.
    - **Interfaces**: Contracts for services and repositories that define how domain operations are carried out.
    - **Models**: Domain entities, data transfer objects, and strongly-typed request/response objects.
    - **Services**: Core business logic implementations (e.g., `AiService`, `VocabularyService`, `UserService`, etc.).

### Infrastructure Layer

**Directory:** `Infrastructure`

- **Purpose:** Responsible for **data persistence** and **integrations** with external systems like AWS DynamoDB.
- **Key Folders/Files:**
    - **Factories**: Contain *factory methods* that convert DynamoDB responses into domain objects.
    - **Repositories**: Implement the repository interfaces defined in `Core.Interfaces`. For example, `UserRepository` interacts with `users` table in DynamoDB.
- **Project:** `Infrastructure.csproj` referencing `Core` so it can implement its interfaces.

### Lambda Layer

**Directory:** `Lambda`

- **Purpose:** Host **AWS Lambda function handlers** that use the domain services from `Core`. They are annotated using the **AWS Lambda Annotations** library for more readable and maintainable code.
- **Sub-Folders:**
    - **LambdaStartup**: Contains `Function.cs` with actual Lambda endpoints (annotated with `[LambdaFunction]` and `[HttpApi(...)]` attributes) and `Startup.cs` for dependency injection configuration.
    - **PreSignUp**: A specialized Lambda that can be triggered by Cognito events for user pre-signup logic.

---

## Design Decisions

1. **Layered Architecture**  
   We adopted a **separation of concerns** approach:
    - **Core**: Business logic and domain models.
    - **Infrastructure**: External data sources and integrations.
    - **Lambda**: AWS-specific entry points using annotations.

2. **Interfaces for Loose Coupling**  
   Each service in `Core` depends on **interfaces** rather than concrete classes, making the code easy to test and flexible to change.

3. **Minimal Framework Coupling**  
   The domain and business logic do not contain any AWS or third-party specifics—these are handled in the `Infrastructure` layer and injected via interfaces. This improves testability.

4. **DynamoDB for Data Persistence**  
   DynamoDB’s **fast and scalable** nature suits the low-latency, serverless environment.

5. **Native AOT Compilation**  
   Targeting `.NET 8` with **Native AOT** helps **reduce cold start** times and **improve performance**—critical for serverless workloads.

6. **Source Generator for JSON Serialization**  
   The `SourceGeneratorLambdaJsonSerializer<CustomJsonSerializerContext>` is used to avoid reflection overhead, improving speed and reducing package size.

---

## Infrastructure Details

### AWS Components

- **Lambda Functions**:  
  Defined in the `Lambda` folder. Deployed via AWS SAM.

- **DynamoDB Tables**:
    - `users`
    - `user_languages`
    - `vocabulary`
    - `allowed_vocabulary`

- **Other Services**:
    - **Amazon Secrets Manager**: Holds API keys for external integrations.
    - **Amazon Cognito**: used for user management and triggers such as `PreSignUp`.

### Azure Components

- **Azure Text Translation**: For text translation tasks.
- **Azure Cognitive Services (Speech)**: Used to issue tokens for text-to-speech or speech-related tasks.

### OpenAI

- Used via an HTTP client for ChatGPT-based requests.

---

## Flow of Execution

1. **HTTP Request or Cognito Trigger** arrives in the **Lambda Function**.
2. The function (in `Lambda/LambdaStartup/Function.cs`) is annotated with `[LambdaFunction]` and `[HttpApi(...)]`, telling AWS Lambda Annotations how to expose the method.
3. The function obtains the user’s **auth token**, parses it with `AuthHelper.ParseToken(...)`.
4. Depending on the endpoint:
    - The relevant **service** in `Core/Services` is called (e.g., `UserService`, `VocabularyService`, `AiService`, etc.).
5. The service calls the appropriate **repository** in `Infrastructure/Repositories` if data persistence is needed.
6. The repository queries or updates **DynamoDB**.
7. The result (entity or data) is returned to the service, then ultimately to the Lambda function, which uses `ResponseHelper` to create a JSON API response.
8. This response is returned to the caller via AWS API Gateway.

---

## Native AOT & AWS Lambda Annotations

### Native AOT
- **Native AOT** compiles the .NET IL code to optimized machine code ahead-of-time.
- This **significantly reduces** the cold-start duration in serverless environments.
- It also helps produce **smaller artifacts** than traditional `IL -> JIT` runtime deployments.

### AWS Lambda Annotations
- Simplifies function setup, removing the need for `FunctionHandler` boilerplate.
- Each method can be annotated with `[LambdaFunction]` or `[HttpApi(...)]`, automatically configuring routing, HTTP methods, etc.
- Integrates seamlessly with the `Startup.cs` approach, allowing for **dependency injection**.

---

## Testing Strategy

Given the layered approach and heavy reliance on **interfaces**, you can write tests at various levels:

1. **Unit Tests (Core)**
    - Test each **service** (e.g., `UserService`, `VocabularyService`, `AiService`) by injecting **mock/fake** repositories or external API clients.
    - Validate business logic in isolation (e.g., verifying sentence generation logic, vocabulary box leveling, etc.).

2. **Integration Tests (Infrastructure)**
    - Test the **repositories** with a local version of DynamoDB (e.g., DynamoDB Local) or a testing environment in AWS.
    - Ensure that queries/updates behave as expected.

3. **End-to-End Tests**
    - Deploy to a staging environment and make **HTTP calls** (or Cognito triggers) to the Lambdas to ensure the entire system is functioning.

**Why It Works**:
- Because the domain logic in `Core` only depends on interfaces, you can swap out `Infrastructure` classes for mocks.
- The same holds for external clients (e.g., ChatGPT, Azure). The code calls an **interface** `IChatGptService`, so you can replace it with a test double that doesn’t hit the real API.

---

## Why This Design Is Robust

1. **Scalability**: DynamoDB’s design and serverless compute scale up and down easily.
2. **Maintainability**: The separate **Core** (business logic) and **Infrastructure** (data logic) keep the codebase clean.
3. **Testability**: Thanks to the interface-based architecture and minimal coupling, each component can be tested independently.
4. **Performance**: **Native AOT** optimizes cold starts, especially beneficial for Lambdas that may be invoked infrequently.
5. **Extensibility**: As new features are introduced (e.g., additional AI providers), the pattern of injecting new services is straightforward.

---

## Pipeline & CI/CD

A GitHub Actions workflow (`.github/workflows/pipeline.yaml`) defines:

1. **Build and Package**
    - Uses `sam build --parallel` for each Lambda.
    - Packages artifacts to S3.

2. **Deploy to AWS**
    - Assumes a specific role in AWS to deploy to a testing environment (`sam deploy`).

3. **Terraform**
    - An optional block for applying/destroying additional infrastructure using Terraform, integrated with the same AWS credentials.

This approach means:
- Every push to `main` or a feature branch triggers a new build.
- Artifacts are stored in an S3 bucket, then SAM deploy is invoked.
- On feature branch deletion, the Terraform pipeline can also destroy infra if configured so.

---

# Conclusion

This project architecture exemplifies a **clean and maintainable** solution for serverless applications:

- **Layered** architecture that strictly separates concerns.
- **Native AOT** for performance.
- **AWS Lambda Annotations** for clean, attribute-based function declarations.
- **Testability** by relying on interfaces and dependency injection.
- **Robust** pipeline for CI/CD with GitHub Actions and AWS SAM/Terraform integration.

We hope this design provides a solid foundation for further development and maintainability. Feel free to contribute improvements or ask questions!