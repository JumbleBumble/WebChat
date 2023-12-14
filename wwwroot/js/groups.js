"use strict";



var currentUsername = null;

document.getElementById("createGrp").addEventListener("click", function (event) {
    var usersInput = document.getElementById("groupUsers").value;
    if (usersInput) {
        var userNames = usersInput.split(',');
        if (userNames.length >= 1) {
            userNames.push(currentUsername);

            fetch('/api/Groups/CreateGroup', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(userNames),
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`HTTP error! Status: ${response.status}`);
                    }
                    return response;
                })
                .then(_ => {
                    location.reload();
                })
                .catch(error => {
                    console.error('Error creating group:', error);
                });
        }
    }
});


function EnterName(event) {
    var NewName = event.target.value;
    var groupID = event.target.id.replace("name ", "")
    fetch(`/api/Groups/Name/${groupID}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(NewName),
    })
        .catch(error => {
            console.error('Error renaming group:', error);
        });
}



function OpenGroup(event) {
    var button = event.target;
    var groupID = button.id.replace("open ", "")
    window.location.href = `Groups/Group/${groupID}`;
}

function DelGroup(event) {
    var button = event.target;
    var groupID = button.id.replace("del ", "")
    fetch(`/api/Groups/${groupID}`, {
        method: 'DELETE'
    })
        .then(response => response)
        .then(data => {
            var groupElement = document.getElementById(groupID);
            if (groupElement) {
                groupElement.remove();
            }
        })
        .catch(error => console.error('Error deleting group:', error));
}






document.addEventListener('DOMContentLoaded', function () {
    currentUsername = document.getElementById('CurrentUsername').value;
});