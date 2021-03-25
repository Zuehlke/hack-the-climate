export async function renderDiagram(element, data) {
    const height = 350;
    const width = 600;
    const color = d3.scaleOrdinal(d3.schemeCategory10);

    const smallestConfidenceScore = data.nodes.map(n => n.confidenceScore).reduce((a, b) => Math.min(a, b));
    const largestConfidenceScore = data.nodes.map(n => n.confidenceScore).reduce((a, b) => Math.max(a, b));
    const radiusScale = d3.scaleLinear()
        .domain([smallestConfidenceScore, largestConfidenceScore])
        .range([3, 10]);

    const index = new Map(data.nodes.map(d => [d.id, d]));
    const links = data.links.map(d => Object.assign(Object.create(d), {
        source: index.get(d.source),
        target: index.get(d.target)
    }));

    const svg = d3.select(element);
    svg.selectAll("*").remove();

    const layout = cola.d3adaptor(d3)
        .size([width, height])
        .nodes(data.nodes)
        .links(links)
        .jaccardLinkLengths(40, 0.7)
        .start(30);

    const link = svg.append("g")
        .attr("stroke", "#999")
        .attr("stroke-opacity", 0.6)
        .selectAll("line")
        .data(links)
        .enter().append("line")
        .attr("stroke-width", d => Math.sqrt(d.value));

    const node = svg.append("g")
        .attr("stroke", "#fff")
        .attr("stroke-width", 1.5)
        .selectAll("circle")
        .data(data.nodes)
        .enter().append("circle")
        .attr("r", d => radiusScale(d.confidenceScore))
        .attr("fill", "#3a0647")
        .call(layout.drag);

    node.append("title")
        .text(d => d.title);

    layout.on("tick", () => {
        link
            .attr("x1", d => d.source.x)
            .attr("y1", d => d.source.y)
            .attr("x2", d => d.target.x)
            .attr("y2", d => d.target.y);

        node
            .attr("cx", d => d.x)
            .attr("cy", d => d.y);
    });

    // TODO invalidation is undefined
    // invalidation.then(() => layout.stop());

    return svg.node();
}