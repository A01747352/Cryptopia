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
ChartJS.defaults.color = 'var(--text-muted)';
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
        borderColor: 'var(--border)',
        drawBorder: false
      },
      ticks: {
        padding: 10,
        color: 'var(--text-muted)',
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
        color: 'var(--text-muted)',
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
      backgroundColor: 'var(--bg-card)',
      titleColor: 'var(--text-light)',
      bodyColor: 'var(--text-light)',
      borderColor: 'var(--primary)',
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
      usePointStyle: true,
      callbacks: {
        label: function(context) {
          const label = context.dataset.label || '';
          // Fix: Ensure the value is a number before calling toFixed
          let value = context.raw;
          let displayValue;
          
          // Check if value is a number or can be converted to one
          if (typeof value === 'number') {
            displayValue = value.toFixed(2);
          } else {
            // If it's not a number, try to convert it or use a default
            displayValue = parseFloat(value) ? parseFloat(value).toFixed(2) : '0.00';
          }
          
          return `${label}: ${displayValue}`;
        }
      }
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
        borderColor: 'var(--border)',
        drawBorder: false
      },
      ticks: {
        padding: 10,
        color: 'var(--text-muted)',
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
        color: 'var(--text-muted)',
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
      backgroundColor: 'var(--bg-card)',
      titleColor: 'var(--text-light)',
      bodyColor: 'var(--text-light)',
      borderColor: 'var(--primary)',
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
      usePointStyle: true,
      callbacks: {
        label: function(context) {
          const label = context.dataset.label || '';
          // Fix: Ensure the value is a number before calling toFixed
          let value = context.raw;
          let displayValue;
          
          // Check if value is a number or can be converted to one
          if (typeof value === 'number') {
            displayValue = value.toFixed(2);
          } else {
            // If it's not a number, try to convert it or use a default
            displayValue = parseFloat(value) ? parseFloat(value).toFixed(2) : '0.00';
          }
          
          return `${label}: ${displayValue}`;
        }
      }
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
        color: 'var(--text-muted)',
        padding: 20,
        font: {
          size: 12
        },
        usePointStyle: true,
        boxWidth: 8
      }
    },
    tooltip: {
      backgroundColor: 'var(--bg-card)',
      titleColor: 'var(--text-light)',
      bodyColor: 'var(--text-light)',
      borderColor: 'var(--primary)',
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
          // Fix: Ensure the value is a number before calling toFixed
          let value = context.raw;
          let displayValue;
          
          // Check if value is a number or can be converted to one
          if (typeof value === 'number') {
            displayValue = value.toFixed(2);
          } else {
            // If it's not a number, try to convert it or use a default
            displayValue = parseFloat(value) ? parseFloat(value).toFixed(2) : '0.00';
          }
          
          return `${label}: ${displayValue}`;
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
  '#3b82f6', // Blue (primary)
  '#6366f1', // Indigo (secondary)
  '#8b5cf6', // Violet
  '#ec4899', // Pink
  '#10b981', // Emerald (success)
  '#f59e0b', // Amber (warning)
  '#64748b', // Slate
  '#ef4444', // Red (danger)
  '#06b6d4', // Cyan
  '#84cc16'  // Lime
];

export { barChartOptions, lineChartOptions, doughnutOptions, modernPalette };