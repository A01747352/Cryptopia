import React, { useState } from 'react';
import './Settings.css';

const Settings = () => {
  const [settings, setSettings] = useState({
    notifications: true,
    darkMode: true,
    language: 'en',
    autoSave: true,
    showOnboarding: false
  });

  const handleChange = (e) => {
    const { name, type, checked, value } = e.target;
    setSettings({
      ...settings,
      [name]: type === 'checkbox' ? checked : value
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    // Aquí se implementaría la lógica para guardar la configuración
    console.log('Settings saved:', settings);
    alert('Settings saved successfully!');
  };

  return (
    <div className="settings-container">
      <h1>Settings</h1>
      
      <div className="settings-card">
        <form onSubmit={handleSubmit}>
          <div className="settings-section">
            <h2>General</h2>
            
            <div className="setting-item">
              <label>
                <span>Language</span>
                <select 
                  name="language" 
                  value={settings.language}
                  onChange={handleChange}
                >
                  <option value="en">English</option>
                  <option value="es">Spanish</option>
                  <option value="fr">French</option>
                </select>
              </label>
            </div>
            
            <div className="setting-item">
              <label>
                <span>Dark Mode</span>
                <input 
                  type="checkbox" 
                  name="darkMode"
                  checked={settings.darkMode}
                  onChange={handleChange}
                />
              </label>
            </div>
          </div>
          
          <div className="settings-section">
            <h2>Notifications</h2>
            
            <div className="setting-item">
              <label>
                <span>Enable Notifications</span>
                <input 
                  type="checkbox" 
                  name="notifications"
                  checked={settings.notifications}
                  onChange={handleChange}
                />
              </label>
            </div>
          </div>
          
          <div className="settings-section">
            <h2>Content</h2>
            
            <div className="setting-item">
              <label>
                <span>Auto-save Changes</span>
                <input 
                  type="checkbox" 
                  name="autoSave"
                  checked={settings.autoSave}
                  onChange={handleChange}
                />
              </label>
            </div>
            
            <div className="setting-item">
              <label>
                <span>Show Onboarding for New Users</span>
                <input 
                  type="checkbox" 
                  name="showOnboarding"
                  checked={settings.showOnboarding}
                  onChange={handleChange}
                />
              </label>
            </div>
          </div>
          
          <button type="submit" className="save-btn">Save Settings</button>
        </form>
      </div>
    </div>
  );
};

export default Settings;