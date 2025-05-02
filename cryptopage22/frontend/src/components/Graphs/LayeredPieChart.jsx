import React, { useEffect, useRef } from 'react';
import * as d3 from 'd3';

const LayeredPieChart = ({x, y, z, width = 250, height = 250 }) => {
  const svgRef = useRef(null);

  useEffect(() => {
    if (svgRef.current) {
      d3.select(svgRef.current).selectAll("*").remove();

      const data = [
        {value: x, color: "#58b3cd", thickness: 110 },
        {value: y, color: "#8cc3d2", thickness: 100 },
        {value: z, color: "#c2cad4", thickness: 90 }
      ];

      const innerR = 60;

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
    }
  }, [width, height]);

  return (
    <svg
      ref={svgRef}
      className="w-full h-full"
      width="100%"
      height="100%"
    />
  );
};

export default LayeredPieChart;
