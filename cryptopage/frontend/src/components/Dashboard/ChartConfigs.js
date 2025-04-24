//ChartConfig.js
// Modificaciones para Charts.js para estilos mejorados
import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    BarElement,
    ArcElement,
    Title,
    Tooltip,
    Legend
  } from 'chart.js';
  
  ChartJS.register(
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    BarElement,
    ArcElement,
    Title,
    Tooltip,
    Legend
  );
  
// Configuración global para Chart.js - Añadir en el Dashboard.jsx
ChartJS.defaults.color = '#a0a0a0';
ChartJS.defaults.font.family = "'Inter', 'Helvetica', 'Arial', sans-serif";

// Ejemplo de configuración para gráficos de barras
const barChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  scales: {
    y: {
      beginAtZero: true,
      grid: {
        color: 'rgba(255, 255, 255, 0.05)',
        borderColor: 'rgba(255, 255, 255, 0.1)',
        drawBorder: false
      },
      ticks: {
        padding: 10,
        color: '#a0a0a0',
        font: {
          size: 11
        }
      }
    },
    x: {
      grid: {
        display: false
      },
      ticks: {
        padding: 10,
        color: '#a0a0a0',
        font: {
          size: 11
        }
      }
    }
  },
  plugins: {
    legend: {
      display: false
    },
    tooltip: {
      backgroundColor: 'rgba(30, 30, 30, 0.8)',
      titleColor: '#fff',
      bodyColor: '#fff',
      borderColor: 'rgba(108, 99, 255, 0.2)',
      borderWidth: 1,
      padding: 12,
      cornerRadius: 8,
      titleFont: {
        size: 14,
        weight: 'bold'
      },
      bodyFont: {
        size: 13
      },
      displayColors: true,
      boxPadding: 6,
      usePointStyle: true
    }
  },
  animation: {
    duration: 1000,
    easing: 'easeOutQuart'
  }
};

// Ejemplo de configuración para gráficos de línea
const lineChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  scales: {
    y: {
      beginAtZero: true,
      grid: {
        color: 'rgba(255, 255, 255, 0.05)',
        borderColor: 'rgba(255, 255, 255, 0.1)',
        drawBorder: false
      },
      ticks: {
        padding: 10,
        color: '#a0a0a0',
        font: {
          size: 11
        }
      }
    },
    x: {
      grid: {
        display: false
      },
      ticks: {
        padding: 10,
        color: '#a0a0a0',
        font: {
          size: 11
        }
      }
    }
  },
  plugins: {
    legend: {
      display: false
    },
    tooltip: {
      backgroundColor: 'rgba(30, 30, 30, 0.8)',
      titleColor: '#fff',
      bodyColor: '#fff',
      borderColor: 'rgba(76, 175, 80, 0.2)',
      borderWidth: 1,
      padding: 12,
      cornerRadius: 8,
      titleFont: {
        size: 14,
        weight: 'bold'
      },
      bodyFont: {
        size: 13
      },
      displayColors: true,
      boxPadding: 6,
      usePointStyle: true
    }
  },
  elements: {
    point: {
      radius: 4,
      hoverRadius: 6,
      borderWidth: 2,
      hoverBorderWidth: 3
    },
    line: {
      tension: 0.4
    }
  },
  animation: {
    duration: 1000,
    easing: 'easeOutQuart'
  }
};

// Ejemplo de configuración para gráficos de dona
const doughnutOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'right',
      labels: {
        color: '#a0a0a0',
        padding: 20,
        font: {
          size: 12
        },
        usePointStyle: true,
        boxWidth: 8
      }
    },
    tooltip: {
      backgroundColor: 'rgba(30, 30, 30, 0.8)',
      titleColor: '#fff',
      bodyColor: '#fff',
      borderColor: 'rgba(255, 99, 132, 0.2)',
      borderWidth: 1,
      padding: 12,
      cornerRadius: 8,
      titleFont: {
        size: 14,
        weight: 'bold'
      },
      bodyFont: {
        size: 13
      },
      callbacks: {
        label: function(context) {
          const label = context.label || '';
          const value = context.raw || 0;
          return `${label}: ${value.toFixed(2)}`;
        }
      }
    }
  },
  cutout: '70%',
  borderRadius: 6,
  spacing: 2,
  animation: {
    animateRotate: true,
    animateScale: true,
    duration: 1000,
    easing: 'easeOutQuart'
  }
};

// Actualiza los colores para ser más vibrantes y modernos
const modernPalette = [
  '#6C63FF', // Púrpura
  '#FF6384', // Rosa
  '#36A2EB', // Azul
  '#FFCE56', // Amarillo
  '#4CAF50', // Verde
  '#FF9F40', // Naranja
  '#9266CC', // Violeta
  '#FF5252', // Rojo
  '#00E5FF', // Cyan
  '#CDDC39'  // Lima
];

// Este código puede utilizarse en tus componentes de gráficos
// importando estas configuraciones desde un archivo separado