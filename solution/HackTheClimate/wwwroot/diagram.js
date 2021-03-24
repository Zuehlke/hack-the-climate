export async function renderDiagram(element, data) {
    const height = 500;
    const width = 500;
    const color = d3.scaleOrdinal(d3.schemeCategory10);

    const index = new Map(data.nodes.map(d => [d.id, d]));
    const links = data.links.map(d => Object.assign(Object.create(d), {
        source: index.get(d.source),
        target: index.get(d.target)
    }));

    const svg = d3.select(element);

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
        .attr("r", 5)
        .attr("fill", d => color(d.group))
        .call(layout.drag);

    node.append("title")
        .text(d => d.id);

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