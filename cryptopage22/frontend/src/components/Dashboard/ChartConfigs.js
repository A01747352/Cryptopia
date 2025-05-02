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

// Register Chart.js components
const registerComponents = () => {
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
};

// Apply global Chart.js configurations
const applyGlobalConfig = () => {
  registerComponents();
  
  // Global styling
  ChartJS.defaults.color = 'var(--text-muted)';
  ChartJS.defaults.font.family = "'Inter', 'Helvetica', 'Arial', sans-serif";
};

// Bar chart options
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
        color: '#ffffff',
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
        color: '#ffffff',
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
      titleColor: '#ffffff',
      bodyColor: '#ffffff',
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
      usePointStyle: true
    }
  },
  animation: {
    duration: 1000,
    easing: 'easeOutQuart'
  }
};


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
        color: '#ffffff', // texto del eje Y en blanco
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
        color: '#ffffff', // texto del eje X en blanco
        font: {
          size: 11
        }
      }
    }
  },
  plugins: {
    legend: {
      labels: {
        color: '#ffffff' // texto de la leyenda en blanco
      },
      display: false
    },
    tooltip: {
      backgroundColor: 'var(--bg-card)',
      titleColor: '#ffffff', // título del tooltip en blanco
      bodyColor: '#ffffff',  // contenido del tooltip en blanco
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


// Doughnut chart options
const doughnutOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'right',
      labels: {
        color: '#ffffff', // texto de la leyenda en blanco
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
      titleColor: '#ffffff', // título del tooltip en blanco
      bodyColor: '#ffffff',  // contenido del tooltip en blanco
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


// Modern color palette with vibrant colors
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

export default {
  applyGlobalConfig,
  registerComponents,
  barChartOptions,
  lineChartOptions,
  doughnutOptions,
  modernPalette
};