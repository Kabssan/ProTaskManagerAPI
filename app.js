const BASE_URL = window.location.hostname === 'localhost' 
    ? 'http://localhost:5265' 
    : 'https://protaskmanagerapi.onrender.com'; 

const API_URL = `${BASE_URL}/api/tasks`;

async function loadTasks() {

    const loader = document.getElementById('loader'); 
    loader.style.display = 'block';

    try {
        const response = await fetch(API_URL);
        const tasks = await response.json();
        const list = document.getElementById('taskList');
        list.innerHTML = ''; 

        tasks.forEach(task => {
            const li = document.createElement('li');
            // Wenn erledigt, machen wir den Text etwas blasser
            li.style.opacity = task.isCompleted ? '0.6' : '1';

            li.innerHTML = `
                <div style="display: flex; align-items: center; gap: 15px;">
                    <input type="checkbox" 
                           ${task.isCompleted ? 'checked' : ''} 
                           onchange="toggleTask(${task.id}, '${task.title}', ${task.isCompleted})"
                           style="width: 20px; height: 20px; cursor: pointer;">
                    <div class="task-info">
                        <strong style="${task.isCompleted ? 'text-decoration: line-through;' : ''}">${task.title}</strong>
                        <small>${task.description || 'Keine Beschreibung'}</small>
                    </div>
                </div>
                <button class="delete-btn" onclick="deleteTask(${task.id})">Löschen</button>
            `;
            list.appendChild(li);
        });
    } catch (error) {
        console.error('Fehler:', error);
    } finally {
        loader.style.display = 'none';              
    }
}

// NEU: Funktion zum Umschalten des Status (Erledigt / Offen)
async function toggleTask(id, title, currentStatus) {
    const updatedTask = {
        id: id,
        title: title,
        isCompleted: !currentStatus // Wir kehren den Status um
    };

    try {
        const response = await fetch(`${API_URL}/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(updatedTask)
        });

        if (response.ok) {
            loadTasks(); // Liste aktualisieren
        }
    } catch (error) {
        console.error('Fehler beim Aktualisieren:', error);
    }
}

async function addTask() {
    const titleInput = document.getElementById('taskTitle');
    const descInput = document.getElementById('taskDesc'); // NEU

    if (!titleInput.value) {
        alert("Bitte gib einen Titel ein!");
        return;
    }

    const newTask = {
        title: titleInput.value,
        description: descInput.value, // NEU: Wert aus dem zweiten Feld
        isCompleted: false
    };

    try {
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(newTask)
        });

        if (response.ok) {
            titleInput.value = ''; 
            descInput.value = ''; // Eingabefelder leeren
            loadTasks();
        }
    } catch (error) {
        console.error('Fehler:', error);
    }
 
}

async function deleteTask(id) {
    if (!confirm("Löschen?")) return;
    await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
    loadTasks();
}

loadTasks();