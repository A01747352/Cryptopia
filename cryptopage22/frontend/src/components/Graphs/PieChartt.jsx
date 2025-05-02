import React, { useEffect, useRef } from 'react';
import * as d3 from 'd3';

const LayeredPieChart = () => {
    const svgRef = useRef(null);
    const legendRef = useRef(null);

    useEffect(() => {
        if (svgRef.current && legendRef.current) {
            d3.select(svgRef.current).selectAll("*").remove();
            d3.select(legendRef.current).selectAll("*").remove();

            const data = [
                { label: "Calls", value: 4, color: "#58b3cd", thickness: 130 },
                { label: "Work Time", value: 3, color: "#8cc3d2", thickness: 115 },
                { label: "Meeting", value: 2, color: "#c2cad4", thickness: 100 }
            ];

            const innerR = 120;
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
                    .style("display", "flex")
                    .style("alignItems", "center")
                    .style("marginBottom", "8px");

                item.append("div")
                    .style("width", "14px")
                    .style("height", "14px")
                    .style("backgroundColor", d.color)
                    .style("marginRight", "8px")
                    .style("borderRadius", "4px");

                item.append("span")
                    .style("color", "#e5e7eb")
                    .style("fontSize", "14px")
                    .text(d.label);
            });
        }
    }, [width, height]);

    return (
        <div className="bg-gray-900 rounded-2xl shadow-xl overflow-hidden w-full max-w-xl">
            <div className="flex flex-row items-center justify-between p-4">
                {/* Leyenda */}
                <div ref={legendRef} className="w-1/3 pl-4"></div>

                {/* Gráfico */}
                <div className="w-2/3 flex items-center justify-center">
                    <svg ref={svgRef} className="w-full h-auto"></svg>
                </div>
            </div>
        </div>
    );
};

export default LayeredPieChart;
