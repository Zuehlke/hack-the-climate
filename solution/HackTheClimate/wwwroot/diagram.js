export async function clearDiagram(element) {
    const svg = d3.select(element);
    svg.selectAll("*").remove();
}

export async function renderDiagram(element, data) {
    const nodeColor = "grey";

    console.log(data);

    const height = 900;
    const width = 900;

    const smallestConfidenceScore = data.nodes.map(n => n.confidenceScore).reduce((a, b) => Math.min(a, b));
    const largestConfidenceScore = data.nodes.map(n => n.confidenceScore).reduce((a, b) => Math.max(a, b));
    const radiusScale = d3.scaleLinear()
        .domain([smallestConfidenceScore, largestConfidenceScore])
        .range([10, 30]);

    const smallestSimilarityScore = data.links.map(l => l.similarityScore).reduce((a, b) => Math.min(a, b));
    const largestSimilarityScore = data.links.map(l => l.similarityScore).reduce((a, b) => Math.max(a, b));
    const linkColorScale = d3.scaleSequential([smallestSimilarityScore - ((largestSimilarityScore - smallestSimilarityScore) / 3), largestSimilarityScore], d3.interpolateGreys);
    const selectedLinkColorScale = d3.scaleSequential([smallestSimilarityScore - ((largestSimilarityScore - smallestSimilarityScore) / 3), largestSimilarityScore], d3.interpolateBlues);
    const linkWidthScale = d3.scaleLinear([smallestSimilarityScore, largestSimilarityScore], [5, 10]);

    const simulation = d3.forceSimulation()
        .force("link", d3.forceLink().id(function(d) { return d.id; }))
        .force("charge", d3.forceManyBody().strength(-200))
        .force("center", d3.forceCenter(width / 2, height / 2));

    const svg = d3.select(element);
    svg.selectAll("*").remove();

    const link = svg.append("g")
        // .attr("stroke-opacity", 0.6)
        .selectAll("line")
        .data(data.links)
        .enter().append("line")
        .attr("stroke-width", d => linkWidthScale(d.similarityScore))
        .attr("stroke", d => linkColorScale(d.similarityScore))
        .on("click", function(d) { window.location.href = "/compare/" + d.source.id + "/" + d.target.id });

    const node = svg.append("g")
        .attr("stroke", "#fff")
        .attr("stroke-width", 1.5)
        .selectAll("g")
        .data(data.nodes)
        .enter().append("g");

    var nodeInformationDiv = d3.select("body").append("div")
        .attr("class", "node-information")
        .style("opacity", 0);

        const onMouseOver = (function () {
        return function(node) {

            var xCord = d3.event.pageX;
            var yCord = d3.event.pageY;

            DotNet.invokeMethodAsync('HackTheClimate', 'GetDetails', node.id)
                .then(data => {
                    nodeInformationDiv.transition().duration(200).style("opacity", 0.9);

                    var flag = '<img src="/flags/' +
                        data.countryCode.toUpperCase() +
                        '.svg" alt="' +
                        data.countryCode.toUpperCase() +
                        '"/>';
                    var topics = [];
                    for (var i = 0; i < Math.min(5, data.topics.length); i++) {
                        topics.push('<span class="badge bg-secondary">' + data.topics[i] + '</span>');
                    }

                    var titleLink = '<a href="/legislation/' +
                        node.id +
                        '" title="' +
                        data.title +
                        '">' +
                        data.title.substr(0, Math.min(data.title.length, 30)) +
                        '</a>';
                    nodeInformationDiv.html('<div>' + flag + "&nbsp;"+titleLink + '<div>' + topics.join() + '</div></div>')
                        .style("left", (xCord) + "px")
                        .style("top", (yCord) + "px");
                });
        }
    })();

    const toggleColor = (function () {
        return function (node) {
            // connected nodes 
            const connectedNodes = data.links.filter(l => l.target.id === node.id || l.source.id === node.id)
                .map(l => {
                    return {
                        id: l.target.id === node.id ? l.source.id : l.target.id,
                        similarityScore: l.similarityScore
                    }
                });

            // nodes
            d3.select(this.farthestViewportElement).selectAll("circle").attr("fill", d => {
                let connectedNode = connectedNodes.find(l => d.id === l.id);
                if (d.id === node.id) {
                    return "#08316D";
                } else if (connectedNode) {
                    return selectedLinkColorScale(connectedNode.similarityScore);
                } else {
                    return nodeColor;
                }
            });

            // links
            d3.select(this.farthestViewportElement).selectAll("line").attr("stroke", d => {
                if (d.source.id === node.id || d.target.id === node.id) {
                    return selectedLinkColorScale(d.similarityScore);
                } else {
                    return linkColorScale(d.similarityScore);
                }
            });

            DotNet.invokeMethodAsync('HackTheClimate', 'OnNodeSelected', node.id);
        }
    })();

    svg.on('mouseleave',
        function () {
            nodeInformationDiv.transition().duration(200).style("opacity", 0);
        });

    const circles = node.append("circle")
        .attr("r", d => radiusScale(d.confidenceScore))
        .attr("fill", nodeColor)
        .on("click", toggleColor)
        .on("mouseenter", onMouseOver)
        .on("mouseleave", function() {
            nodeInformationDiv.transition().duration(200).style("opacity", 0);
        })
        .call(d3.drag()
            .on("start", d => dragstarted(d, simulation))
            .on("drag", d => dragged(d))
            .on("end", d => dragended(d, simulation)));

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

        const radius = 10;

        node
            .attr("transform",
                function (d) {
                    return "translate(" + d.x + "," + d.y + ")";
                })
            .attr("cx", function (d) { return d.x = Math.max(radius, Math.min(width - radius, d.x)); })
            .attr("cy", function (d) { return d.y = Math.max(radius, Math.min(height - radius, d.y)); });
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
