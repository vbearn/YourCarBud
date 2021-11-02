# YourCarBud

A .NET 5 application to be Your Car Buddy for ordering and delivery of used cars!

## 1. Getting Started

Clone the repository in your local machine.

- Open the `YourCarBud.sln` solution from `src` folder using Visual Studio 2019+
- Make sure you have a valid `ConnectionString` in `appsettings.Development.json`.
- Hit F5:
- [ Or simply navigate to `src\Api\YourCarBud.WebApi` folder in the terminal and type `dotnet run` ]

You should be able to browse the Swagger interface of the application by using the URL http://localhost:5005

See the last section of this file to learn how to test the app using Postman.

## 2. Business Flow

The applications exposes endpoints to

- Create an order
- Create Contact Details for the order, and update its status in a workflow, from Pending -> Start -> Success/Fail
- Process Delivery Appointment and Payment for the order, simultaneously, and update their statuses in a workflow, from Pending -> Start -> Success/Fail
- Mark the status of the whole order to Success (when all steps are succeeded) or Fail (when any step fails)

Below are the explicit details of each endpoint alongside their Verb and sample Payload to be executed (via tools like Postman)

### 2.1. Create the Order

First, an Order should be created. The Id of this Order will be used in the next steps.

| Describe                                  | Endpoint      | Verb | Json Payload             |
| ----------------------------------------- | ------------- | ---- | ------------------------ |
| Create a new order with a specific amount | `/api/orders` | POST | `{ "totalAmount": 100 }` |

A newly create order will have 3 default Steps, all initialized with Pending status. Sample:

```
{
    "id": "ad0ef9a0-0fe8-47d0-8b58-08d99d81c48a",
    "createdAt": "2021-11-02T05:51:32.3135542+08:00",
    "completedAt": "0001-01-01T00:00:00+00:00",
    "totalAmount": 100.00,
    "errorMessage": null,
    "status": "Pending",
    "steps": [
        {
            "id": "0ca76874-ba13-4e98-1888-08d99d81c499",
            "stepName": "ContactDetails",
            "status": "Pending"
        },
        {
            "id": "860f754b-9571-4008-1889-08d99d81c499",
            "stepName": "ProcessPayment",
            "status": "Pending"
        },
        {
            "id": "a3b9c79a-f81f-42da-188a-08d99d81c499",
            "stepName": "ProcessDeliveryAppointment",
            "status": "Pending"
        }
    ]
}
```

### 2.2. Update the Status of ContactDetails step

Next, the first Order Step (ContactDetails) should be promoted to Success. This happens via calling a dedicated endpoint to promote its status, one by one, from Pending -> Start -> Success/Fail

| Describe                                                    | Endpoint                                             | Verb | Json Payload                                                |
| ----------------------------------------------------------- | ---------------------------------------------------- | ---- | ----------------------------------------------------------- |
| Promote ContactDetails step (from initial Pending) to Start | `/api/orders/{orderId} /steps/contactDetails/status` | PUT  | `{ "status": "Start" }`                                     |
| Promote ContactDetails step to Success                      | `/api/orders/{orderId} /steps/contactDetails/status` | PUT  | `{ "status": "Success", "Email":"sampleEmail@doamin.com" }` |
| Abandon ContactDetails step and mark the Status as Fail     | `/api/orders/{orderId} /steps/contactDetails/status` | PUT  | `{ "status": "Fail", "message": "Reason for failure" }`     |

### 2.3. Update the Status of ProcessPayment and ProcessDeliveryAppointment steps

After that, both remaining Order Steps (ProcessPayment and ProcessDeliveryAppointment) should be promoted to Success. Their status can be promoted simultaneously, regardless of the other one's state. When both of these steps have gone through the Pending -> Start -> Success/Fail journey, the Order will proceed to the final step.

| Describe                                                                  | Endpoint                                                         | Verb  | Json Payload                                                                         |
| ------------------------------------------------------------------------- | ---------------------------------------------------------------- | ----- | ------------------------------------------------------------------------------------ |
| Promote ProcessPayment step (from initial Pending) to Start               | `/api/orders/{orderId} /steps/processPayment/status`             | PUT   | `{ "status": "Start" }`                                                              |
| Promote ProcessPayment step to Success                                    | `/api/orders/{orderId} /steps/processPayment/status`             | PUT   | `{ "status": "Success", "Amount":"100000" }`                                         |
| Abandon ProcessPayment step and mark the Status as Fail                   | `/api/orders/{orderId} /steps/processPayment/status`             | PUT   | `{ "status": "Fail", "message": "Reason for failure" }`                              |
| -----                                                                     | -----                                                            | ----- | -----                                                                                |
| Promote ProcessDeliveryAppointment Status (from initial Pending) to Start | `/api/orders/{orderId} /steps/processDeliveryAppointment/status` | PUT   | `{ "status": "Start" }`                                                              |
| Promote ProcessDeliveryAppointment Status to Success                      | `/api/orders/{orderId} /steps/processDeliveryAppointment/status` | PUT   | `{ "status": "Success", "AppointmentDateTime":"2021-11-02T05:51:32.3135542+08:00" }` |
| Abandon ProcessDeliveryAppointment step and mark the Status as Fail       | `/api/orders/{orderId} /steps/processDeliveryAppointment/status` | PUT   | `{ "status": "Fail", "message": "Reason for failure" }`                              |

It should be noted that the `ContactDetail`, `DeliveryAppointment`, `Payment` entities are created when their respective steps are marked as Success.

### 2.4. Update the Status of the Order based on the progress of its steps

Finally, the Order will be marked as Success (when all the steps are succeeded) or Fail (when any step fails).

Additionally, the `CompletedAt` field is set for a successful Order.

The progress of the Order and its child Order Steps can be monitored via endpoints to fetch the Order's latest state:

| Describe             | Endpoint                | Verb | Json Payload |
| -------------------- | ----------------------- | ---- | ------------ |
| Get all orders       | `/api/orders`           | GET  | N/A          |
| Get a specific order | `/api/orders/{orderId}` | GET  | N/A          |

## 3. Assumptions and Thought Processes

### 3.1. Selecting between Guid, long, or string as Entity Id

Whilst there are many arguments and debates on pros and cons of each Data Type for using as the Entity Id (15000+ [debates on stackoverflow alone](https://www.google.com/search?q=database+id+guid+vs+int+vs+string+site:stackoverflow.com&rlz=1C1CHBF_enMY889MY889&sxsrf=AOaemvKJwmXa0gHjik5ntwD0pBKCgZkB9g:1635835306439&sa=X&ved=2ahUKEwiX_OayifnzAhUXeCsKHVu4CRcQrQIoBHoECDUQBQ&biw=1396&bih=656&dpr=1.38)), the current application currently uses `Guid`s as the primary Data Type for all entities due its simplicity of usage and seeding, specially when it is time to "Transform the Architecture Into Microservices".

This, of course, is a un-educated assumption yet, due to currently-unknown production requirements, and the progress path of application's architecture in the future.

### 3.2. Adding new Order Steps elegantly, without much hard-coding

Extra care has been given into making the Order Steps as dynamic as possible, in order to facilitate the (possible) addition of new Steps into the application.

To add a new Order Step (for example an step called CustomerApproval, which is done after all the current steps and deals with the customer safely receiving and approving of the quality of the product):

3.2.1. Add a new class inheriting from `IOrderStepWorkflowBehaviour`:

```
public class CustomerApprovalWorkflowBehaviour : IOrderStepWorkflowBehaviour
{
    public const string _OrderStepName = "CustomerApproval";
    public string OrderStepName => _OrderStepName;

    // this indicates the additional information received when marking the step as Success
    public Type DetailCreationArgsDtoType => typeof(CustomerApprovalCreationArgsDto);

    // other class fields and properties

    // constructor and DI args
    // ... //


    public void ValidateStatusUpdate(Order order, Statuses statusToBeUpdatedInto)
    {
        // specify how the Workflow of the current step should progress, for example
        // it should be Started only after all the previous steps are Succeeded.
        // and also,for example, validate that we can not jump from Pending to Fail, without going through Start first.
    }


    public async Task AfterStatusUpdate(Guid orderId, OrderStepStatusUpdateModel updateModel)
    {
          // synchronize and update all the dependent entities, after each step.
          // for example, a new instance of CustomerApproval entity should be created
          // when this step succeeds, holding extra fields for `CustomerScoreRating` and `CustomerFeedback`
    }
}

```

3.2.2. Now wire up all the DI configurations in `OrderStepModuleConfiguration` and now the new step is ready for usage!

3.2.3. Note that a new Controller Endpoint is **automatically** created for updating the status of this OrderStep, without the need to specify anything manually.

It can be accessed via executing a PUT against `/api/orders/{orderId}/steps/customerApproval/status` URI.

### 3.3. Allowed states for the Workflow

Currently the allowed traversal paths for the workflow of the Order Steps are:

- Each step can ONLY go from Pending -> Start -> Success/Fail
- Steps can NOT jump from Pending -> Success/Fail without going through Start first.
- Steps can NOT be demoted, i.e, an Status change from Start -> Pending is NOT allowed.
- Steps can NOT be updated anymore when they have been landed into Success/Fail state.
- Steps can NOT be updated anymore when the whole Order has landed into Success/Fail state.
- ProcessDeliveryAppointment and ProcessPayment steps can ONLY be updated when the ContactDetails step is marked as Success.

### 3.4. Choosing URI and Verb for Status Update endpoint

Which one is correct?

- Executing a PATCH on `/api/orders/{orderId}/steps/{stepName}`, or
- Executing a PUT on `/api/orders/{orderId}/steps/{stepName}/status`
- Executing a POST on action-like resource `/api/orders/{orderId}/steps/{stepName}/promoteStatus`

All three can be considered as correct from a RESTful point of view. It depends on how the application looks at the resource.

This application currently considers the _"Status of the OrderStep"_ as an independent resource by itself.

Hence PUT can be used to modify the _"Status of the OrderStep"_ as a self-reliant entity.

Using PUT also has the added benefit of being Idempotent and also contains simpler payload.

### 3.5. Passing the additional information for Steps when marked as Success

Consider the `Email` field for `ContactDetail`, the `AppointmentDateTime` for `DeliveryAppointment`, and the `Amount` for `Payment`.

- What is the best way to add additional information for each step?
- When should these fields be amended to the Order Step?

Currently the application accepts this extra data exactly when marking the OrderStep as Success, using a dynamically-crafted DTO. (Refer to section 2.3. for the payloads)

This approach has the benefit of being fully automated and dynamic, but lacks a clear specification and validators for the Json Payload. Hence currently a manual validation is being performed on the DTO.

TODO: This can be further improved

- ...
- ...
- ...

### - Tests

Currently a Unit Test project is implemented for the main library.

![](/images/img1.jpg)

TODO: implement full-fledged Postman test for end to end testing of the API
