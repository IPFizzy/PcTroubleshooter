/*
 * Keon Bushman
 * PC Troubleshooter
 * Local Windows Troubleshooting Dashboard
 * Created: 2026
 *
 * This file is part of a local troubleshooting utility designed to run
 * approved diagnostic and support tools on a Windows computer with permission.
 */

// Gets the output panel where command results will be displayed.
const output = document.getElementById("output");

// Maps backend tool categories to the correct section on the page.
const categoryMap = {
    network: document.getElementById("networkTools"),
    windowsTools: document.getElementById("windowsTools"),
    reports: document.getElementById("reportTools"),
    advanced: document.getElementById("advancedTools")
};

/**
 * Loads the approved tool list from the local API and creates one card per tool.
 */
async function loadTools() {
    try {
        const response = await fetch("/api/tools");
        const tools = await response.json();

        tools.forEach(tool => {
            const card = createToolCard(tool);
            const categoryKey = getCategoryKey(tool.category);

            if (categoryMap[categoryKey]) {
                categoryMap[categoryKey].appendChild(card);
            }
        });
    } catch (error) {
        output.textContent = "Error loading tools: " + error;
    }
}

/**
 * Converts the category value from the backend into a frontend section key.
 * Handles both string enum values and numeric enum values.
 *
 * @param {string|number} category The category value returned by the API.
 * @returns {string} The matching frontend category key.
 */
function getCategoryKey(category) {
    if (category === "Network" || category === 0) {
        return "network";
    }

    if (category === "WindowsTools" || category === 1) {
        return "windowsTools";
    }

    if (category === "Reports" || category === 2) {
        return "reports";
    }

    if (category === "Advanced" || category === 3) {
        return "advanced";
    }

    return "network";
}

/**
 * Creates a visual card and button for one troubleshooting tool.
 *
 * @param {object} tool The tool definition returned from the API.
 * @returns {HTMLDivElement} The completed tool card element.
 */
function createToolCard(tool) {
    const card = document.createElement("div");
    card.className = "tool-card";

    const title = document.createElement("h3");
    title.textContent = tool.displayName;

    const description = document.createElement("p");
    description.textContent = tool.description;

    const button = document.createElement("button");
    button.textContent = tool.buttonText ?? "Run / Open";

    if (tool.isAdvanced) {
        button.classList.add("advanced");
    }

    button.addEventListener("click", () => runTool(tool));

    card.appendChild(title);
    card.appendChild(description);
    card.appendChild(button);

    return card;
}

/**
 * Runs the selected troubleshooting tool by calling the local API.
 *
 * @param {object} tool The selected tool definition.
 */
async function runTool(tool) {
    if (tool.isAdvanced) {
        const confirmed = confirm(
            `${tool.displayName} is an advanced tool and may change system settings.\n\nOnly continue if you understand what it does and have permission.`
        );

        if (!confirmed) {
            return;
        }
    }

    output.textContent = `Running: ${tool.displayName}...`;

    try {
        const response = await fetch(`/api/tools/${tool.action}`, {
            method: "POST"
        });

        const result = await response.json();

        let text = "";

        text += result.success ? "Success\n\n" : "Failed\n\n";
        text += result.message ?? "";

        if (result.filePath) {
            text += `\n\nFile: ${result.filePath}`;
        }

        if (result.output) {
            text += `\n\n${result.output}`;
        }

        output.textContent = text;
    } catch (error) {
        output.textContent = "Error running tool: " + error;
    }
}

// Starts the page by loading the available troubleshooting tools.
loadTools();
