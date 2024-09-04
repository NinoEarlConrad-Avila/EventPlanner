using EventPlanner.Models;
using EventPlanner.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EventContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("EventDbContext"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CRUD Endpoints for Event

// Create
app.MapPost("/events", async (EventContext db, NewEvent nEvent) =>
{
    var newEvent = db.Events.Add(new(null, nEvent.EventName, nEvent.Description, nEvent.Date, nEvent.EventDeadline, nEvent.Status));
    await db.SaveChangesAsync();
    return Results.Created($"/events/{newEvent.Entity.EventID}", newEvent.Entity);
})
.WithName("AddEvent")
.WithOpenApi();

// Read
app.MapGet("/events", async (EventContext db) =>
{
    return await db.Events.ToListAsync();
})
.WithName("GetEvents")
.WithOpenApi();

// Read Single Event
app.MapGet("/events/{id:int}", async (EventContext db, int id) =>
{
    var eventItem = await db.Events.FindAsync(id);
    return eventItem is not null ? Results.Ok(eventItem) : Results.NotFound();
})
.WithName("GetEventById")
.WithOpenApi();
    

// Update
app.MapPut("/events/{id:int}", async (EventContext db, int id, NewEvent updatedEvent) =>
{
    var Event = await db.Events.FindAsync(id);

    if (Event is null) return Results.NotFound();

    var updatedEventEntity = Event with
    {
        EventName = updatedEvent.EventName,
        Description = updatedEvent.Description,
        Date = updatedEvent.Date,
        EventDeadline = updatedEvent.EventDeadline,
        Status = updatedEvent.Status
    };

    db.Entry(Event).CurrentValues.SetValues(updatedEventEntity);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Delete
app.MapDelete("/events/{id:int}", async (EventContext db, int id) =>
{
    var eventToDelete = await db.Events.FindAsync(id);
    if (eventToDelete is null) return Results.NotFound();

    db.Events.Remove(eventToDelete);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
