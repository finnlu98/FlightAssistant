import re

def to_pascal_case(code):
    """Converts a country code to PascalCase (e.g., 'US' -> 'Us')"""
    return code.capitalize()

def generate_flag_map(input_file, output_file):
    with open(input_file, 'r') as file:
        lines = file.readlines()

    flag_imports = []
    flag_map_entries = []

    for line in lines:
        # Extract code2 and country name using regex
        match = re.search(r'code2: "(\w+)",.*name: "(.*?)"', line)
        if match:
            code2 = match.group(1)
            country_name = match.group(2)
            pascal_case_code = to_pascal_case(code2)

            flag_imports.append(f'import {{ {pascal_case_code} }} from "react-flags-select";')
            flag_map_entries.append(f'    "{code2}": {pascal_case_code}, // {country_name}')

    # Create the flag map file content
    content = (
        "// Auto-generated flag map file\n\n"
        "import React from \"react\";\n\n"
        + "\n".join(flag_imports)
        + "\n\nconst FlagMap: { [key: string]: React.ComponentType } = {\n"
        + "\n".join(flag_map_entries)
        + "\n};\n\nexport default FlagMap;\n"
    )

    # Write the generated content to the output file
    with open(output_file, 'w') as out_file:
        out_file.write(content)

    print(f"Flag map file generated successfully: {output_file}")

# File paths
input_file = 'Countries.txt'
output_file = 'FlagMap.tsx'

# Generate the flag map
generate_flag_map(input_file, output_file)
