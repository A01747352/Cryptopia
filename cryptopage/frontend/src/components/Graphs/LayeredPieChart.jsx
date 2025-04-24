import React, { useEffect, useRef } from 'react';
import * as d3 from 'd3';

const LayeredPieChart = ({ width = 300, height = 300 }) => {
  const svgRef = useRef(null);
  const legendRef = useRef(null);

  useEffect(() => {
    if (svgRef.current && legendRef.current) {
      // limpiar cualquier contenido previo
      d3.select(svgRef.current).selectAll("*").remove();
      d3.select(legendRef.current).selectAll("*").remove();

      const data = [
        { label: "Calls",     value: 4,  color: "#58b3cd", thickness: 130 },
        { label: "Work Time", value: 3,  color: "#8cc3d2", thickness: 115 },
        { label: "Meeting",   value: 2,  color: "#c2cad4", thickness: 100 }
      ];

      const totalHours = d3.sum(data, d => d.value);
      
      // Dimensiones internas de D3 (viewBox)
      const innerR = 80;

      const svg = d3.select(svgRef.current)
        .attr("viewBox", `0 0 ${width} ${height}`)
        .attr("width", width)
        .attr("height", height)
        .attr("preserveAspectRatio", "xMidYMid meet")
        .append("g")
          .attr("transform", `translate(${width / 2}, ${height / 2})`);

      const pie = d3.pie()
        .sort(null)
        .value(d => d.value);

      const arcs = pie(data);

      // Halo exterior
      svg.selectAll(".halo")
        .data(arcs)
        .join("path")
        .attr("class", "halo")
        .attr("d", d3.arc()
          .innerRadius(d => d.data.thickness)
          .outerRadius(d => d.data.thickness + 8)
          .startAngle(d => d.startAngle)
          .endAngle(d => d.endAngle)
        )
        .attr("fill", d => d.data.color)
        .attr("fill-opacity", 0.20);

      // Sectores principales
      svg.selectAll(".arc")
        .data(arcs)
        .join("path")
        .attr("class", "arc")
        .attr("d", d3.arc()
          .innerRadius(innerR)
          .outerRadius(d => d.data.thickness)
        )
        .attr("fill", d => d.data.color);

      // Leyenda
      const legend = d3.select(legendRef.current);
      data.forEach(d => {
        const item = legend.append("div")
          .attr("class", "flex items-center mb-2");
        
        item.append("div")
          .attr("class", "w-2 h-5 mr-2 rounded")
          .style("background-color", d.color);
        
        item.append("div")
          .attr("class", "text-sm text-gray-300")
          .text(d.label);
      });
    }
  }, [width, height]);

  return (
    <div
      className="bg-gray-900 rounded-lg shadow-lg overflow-hidden w-96"
      style={{ aspectRatio: '3/2' }}
    >
      <div className="flex h-full">
        {/* Lado izquierdo: gráfico */}
        
        
        {/* Lado derecho: leyenda */}
        <div className="w-1/3 pl-8 pt-24 flex flex-col justify-center">
          <div ref={legendRef}></div>
        </div><div className="flex-shrink-0 flex items-center justify-center w-1/2 m-2">
          <svg
            ref={svgRef}
            className="w-full h-full"
          ></svg>
        </div>
      </div>
    </div>
  );
};

export default LayeredPieChart;
