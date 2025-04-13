// Funciones para el dashboard interactivo

// Inicializar gráficos
function initCharts(completionRate, ageData) {
    // Gráfico de tasa de finalización (gráfico de medidor)
    const completionCtx = document.getElementById('completionChart').getContext('2d');
    const completionChart = new Chart(completionCtx, {
      type: 'doughnut',
      data: {
        labels: ['Completado', 'Pendiente'],
        datasets: [{
          data: [completionRate, 100 - completionRate],
          backgroundColor: ['#4CAF50', '#e0e0e0'],
          borderWidth: 0
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '70%',
        plugins: {
          legend: {
            position: 'bottom'
          },
          tooltip: {
            callbacks: {
              label: function(context) {
                return context.label + ': ' + context.parsed + '%';
              }
            }
          }
        },
        animation: {
          animateRotate: true
        }
      }
    });
  
    // Gráfico de distribución por edades
    const ageLabels = ageData.map(item => item.range);
    const ageCounts = ageData.map(item => item.count);
    const ageColors = [
      '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF'
    ];
  
    const ageCtx = document.getElementById('ageChart').getContext('2d');
    const ageChart = new Chart(ageCtx, {
      type: 'bar',
      data: {
        labels: ageLabels,
        datasets: [{
          label: 'Número de Usuarios',
          data: ageCounts,
          backgroundColor: ageColors,
          borderWidth: 1
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            display: false
          },
          tooltip: {
            callbacks: {
              label: function(context) {
                const item = ageData[context.dataIndex];
                return `Usuarios: ${item.count.toLocaleString()} (${item.percentage}%)`;
              }
            }
          }
        },
        scales: {
          y: {
            beginAtZero: true,
            title: {
              display: true,
              text: 'Número de Usuarios'
            }
          },
          x: {
            title: {
              display: true,
              text: 'Rango de Edad'
            }
          }
        }
      }
    });
}
  
// Función para inicializar el mapa mundial (simulado)
function initWorldMap(countryData) {
  const mapContainer = document.getElementById('worldMap');
  
  // En una implementación real, aquí usaríamos una biblioteca como Leaflet o Google Maps
  // Para esta demo, vamos a simular un mapa con información de los países
  
  let mapContent = '<div class="w3-padding">';
  mapContent += '<h4 class="w3-center">Mapa de Distribución de Usuarios</h4>';
  
  // Crear representación visual simple de los países
  countryData.forEach(country => {
    const size = Math.max(30, Math.min(100, country.users / 50));
    const opacity = Math.max(0.5, Math.min(1, country.completion / 100));
    
    mapContent += `
      <div class="w3-padding w3-margin-bottom" style="border-left: 8px solid #3498db; opacity: ${opacity};">
        <strong>${country.country}</strong>: ${country.users.toLocaleString()} usuarios
        <div class="w3-light-grey w3-round">
          <div class="w3-container w3-blue w3-round" style="width:${country.completion}%">
            ${country.completion}%
          </div>
        </div>
      </div>
    `;
  });
  
  mapContent += '<p class="w3-center w3-opacity">Para una visualización más precisa, se implementará un mapa interactivo real.</p>';
  mapContent += '</div>';
  
  mapContainer.innerHTML = mapContent;
}

// Cargar datos de usuarios para la tabla
async function loadUserTable() {
  try {
    // En producción, esto sería una llamada a la API:
    // const response = await fetch('/api/users');
    // const users = await response.json();
    
    // Para la demo, vamos a simular datos
    const users = [
      { id: 1, username: "estudiante123", age: 14, country: "México", progress: 78, last_login: "2025-04-02" },
      { id: 2, username: "aprendo_feliz", age: 9, country: "Colombia", progress: 45, last_login: "2025-04-05" },
      { id: 3, username: "matematicas_pro", age: 16, country: "España", progress: 92, last_login: "2025-04-07" },
      { id: 4, username: "ciencia_divertida", age: 12, country: "Argentina", progress: 67, last_login: "2025-04-01" },
      { id: 5, username: "futuro_ingeniero", age: 15, country: "México", progress: 83, last_login: "2025-04-06" }
    ];
    
    const tableBody = document.getElementById('userTableBody');
    tableBody.innerHTML = '';
    
    users.forEach(user => {
      const row = document.createElement('tr');
      
      // Crear celdas para cada propiedad
      row.innerHTML = `
        <td>${user.id}</td>
        <td>${user.username}</td>
        <td>${user.age}</td>
        <td>${user.country}</td>
        <td>
          <div class="w3-light-grey w3-round-xlarge">
            <div class="w3-container w3-round-xlarge w3-blue" style="width:${user.progress}%">${user.progress}%</div>
          </div>
        </td>
        <td>${user.last_login}</td>
      `;
      
      tableBody.appendChild(row);
    });
    
  } catch (error) {
    console.error('Error al cargar los datos de usuarios:', error);
    document.getElementById('userTableBody').innerHTML = 
      '<tr><td colspan="6" class="w3-center w3-text-red">Error al cargar datos de usuarios</td></tr>';
  }
}