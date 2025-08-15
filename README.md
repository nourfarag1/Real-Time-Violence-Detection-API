# Real-Time AI-Powered Violence Detection System

> A multi-service backend system that analyzes live video streams using a hybrid cloud/local AI pipeline to detect and report violent incidents in real-time.

![Project Demo GIF](./demo.gif)
*Live demo showing a notification being triggered after a violent act is detected.*

---

## About The Project

This project was developed as my B.Sc. Graduation Project, where I acted as the technical lead and system architect for a 5-person team. The core challenge was to design a robust, scalable system capable of ingesting multiple live video streams and using AI to identify potential violence with minimal latency.

The system is architected as a set of containerized microservices orchestrated by a central .NET API. The most significant architectural achievement was designing a hybrid cloud/local pipeline to overcome hardware limitations, processing latency-sensitive tasks locally while leveraging a cloud GPU for complex classification.

### Key Features:
*   **Real-Time Data Pipeline:** Ingests live video streams and processes them into chunks for AI analysis using RabbitMQ and MinIO.
*   **Hybrid AI Processing:** A unique architecture that splits AI tasks between local and cloud resources to balance performance and cost.
*   **Microservice Architecture:** Decoupled, containerized Python services for scalability and maintainability.
*   **Secure RESTful API:** Built with ASP.NET Core for client communication, handling user authentication, camera management, and alert history.
*   **Real-Time Notifications:** Pushes alerts to a client application upon detection of an incident.

## Tech Stack

*   **Backend API:** C#, ASP.NET Core
*   **AI Services:** Python, Flask, OpenCV
*   **Data Pipeline:** RabbitMQ (Message Queue), MinIO (Object Storage)
*   **Stream Management:** Simple-RTMP-Server (SRS)
*   **Containerization:** Docker, Docker Compose
*   **Client (for demo):** Flutter

## System Architecture

![System Architecture Diagram](./architecture.png)

## Getting Started

To get a local copy up and running, follow these steps.

### Prerequisites

*   Docker and Docker Compose
*   .NET 8 SDK
*   Python 3.10

### Installation & Launch

1.  **Clone the repositories:** (This main repository links to the others)
    ```sh
    git clone https://github.com/nourfarag1/Real-Time-Violence-Detection-API.git
    ```
2.  **Configure Environment Variables:**
    *   Navigate to the API project directory.
    *   Create a `.env` file from the provided `.env.example`.
    *   Fill in the necessary variables.
3.  **Launch the System with Docker Compose:**
    ```sh
    docker-compose up --build
    ```
---
