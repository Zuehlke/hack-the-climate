export async function renderDiagram(element, data) {
    const height = 350;
    const width = 600;

    const smallestConfidenceScore = data.nodes.map(n => n.confidenceScore).reduce((a, b) => Math.min(a, b));
    const largestConfidenceScore = data.nodes.map(n => n.confidenceScore).reduce((a, b) => Math.max(a, b));
    const radiusScale = d3.scaleLinear()
        .domain([smallestConfidenceScore, largestConfidenceScore])
        .range([3, 10]);

    const simulation = d3.forceSimulation()
        .force("link", d3.forceLink().id(function(d) { return d.id; }))
        .force("charge", d3.forceManyBody())
        .force("center", d3.forceCenter(width / 2, height / 2));

    const svg = d3.select(element);
    svg.selectAll("*").remove();
    svg.call(d3.zoom().on("zoom",
        function() {
            svg.attr("transform", d3.event.transform)
        }));

    const link = svg.append("g")
        .attr("stroke", "#999")
        .attr("stroke-opacity", 0.6)
        .selectAll("line")
        .data(data.links)
        .enter().append("line")
        .attr("stroke-width", d => Math.sqrt(d.value));

    const node = svg.append("g")
        .attr("stroke", "#fff")
        .attr("stroke-width", 1.5)
        .selectAll("g")
        .data(data.nodes)
        .enter().append("g")

    const circles = node.append("circle")
        .attr("r", d => radiusScale(d.confidenceScore))
        .attr("fill", "#3a0647")
        .call(d3.drag()
            .on("start", d => dragstarted(d, simulation))
            .on("drag", d => dragged(d))
            .on("end", d => dragended(d, simulation)));

    node.append("title")
        .text(d => d.title);

    simulation
        .nodes(data.nodes)
        .on("tick", ticked);

    simulation.force("link")
        .links(data.links);

    function ticked() {
        link
            .attr("x1", function (d) { return d.source.x; })
            .attr("y1", function (d) { return d.source.y; })
            .attr("x2", function (d) { return d.target.x; })
            .attr("y2", function (d) { return d.target.y; });

        node
            .attr("transform", function (d) {
                return "translate(" + d.x + "," + d.y + ")";
            })
    }
}

function dragstarted(d, simulation) {
    if (!d3.event.active) simulation.alphaTarget(0.3).restart();
    d.fx = d.x;
    d.fy = d.y;
}

function dragged(d) {
    d.fx = d3.event.x;
    d.fy = d3.event.y;
}

function dragended(d, simulation) {
    if (!d3.event.active) simulation.alphaTarget(0);
    d.fx = null;
    d.fy = null;
}