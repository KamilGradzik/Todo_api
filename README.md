# ToDo API (WIP)
Simple API developed for small ToDo application. API was created for recruitment task purposes.
It has few basic endpoints like adding, modyfing, getting and deleting. Endpoints are described below.

---

## Endpoints

- **POST /api/todo/tasks/create** - Creates new task, user provides title, description and due date.
  
- **PATCH /api/todo/tasks/update/{id}** - Updates existing task, user provides task Id and modifed title, desc or due date.
  
- **PATCH /api/todo/tasks/change-percentage/{id}** - Changes task percentage completness, user provides task Id and new percentage value.
  
- **PATCH /api/todo/tasks/change-status/{id}** - Marks task status as done or undone, user provides task Id true or false value.
  
- **GET /api/todo/tasks/{id}** - Returns task with provided by user id.
  
- **GET /api/todo/tasks/today** - Returns list of tasks that due date is for today's day.
  
- **GET /api/todo/tasks/tommorow** - Returns list of tasks that due date is for tomorrws's day.
  
- **GET /api/todo/tasks/all** - Returns all of tasks that exists in data base.
  
- **DELETE /api/todo/tasks/delete/{id}** - Deletes task with specifed by user id.
