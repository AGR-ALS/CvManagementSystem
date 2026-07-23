# Setup

## Clone Repository
```
git clone https://github.com/AGR-ALS/CvManagementSystem.git
cd CvManagementSystem/
```

## Enter enviromental variables

Create `.env` files and write variables into them according to `.env_examples` files.

For example:
```
EmailSettings__FromName='App'
EmailSettings__FromEmail='test@gmail.com'
EmailSettings__ToName='Recipient'
EmailSettings__ClientHost='smtp.gmail.com'
EmailSettings__ClientPort=465
EmailSettings__ClientLogin='test@gmail.com'
EmailSettings__ClientPassword='qwerty'
EmailSettings__UseSsl=true

RabbitMqSettings__Host=rabbitmq://rabbitmq
RabbitMqSettings__Username=test
RabbitMqSettings__Password=test
```
Each service contains its own `.env` file, make sure to go through all of them.

## Launch the App

```
docker compose up --build -d
```

### You can access the app at [localhost:3000](http://localhost:3000)

## Premade Admin

You can access premade admin user using credentials:
```
email: admin@admin.com
password: admin
```