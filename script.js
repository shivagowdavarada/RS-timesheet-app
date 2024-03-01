document.getElementById('timesheetForm').addEventListener('submit', function(event) {
    event.preventDefault();

    var formData = new FormData(this);

    fetch('/timesheet/submit', {
        method: 'POST',
        body: JSON.stringify({
            projectName: formData.get('projectName'),
            date: formData.get('date'),
            assignedDate: formData.get('assignedDate'),
            assignPoc: formData.get('assignPoc')
        }),
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(response => response.text())
    .then(message => {
        document.getElementById('message').textContent = message;
    })
    .catch(error => console.error('Error:', error));
});
