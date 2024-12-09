import re

# Input and output file names
input_file = "Countries.txt"
output_file = "insert_countries.sql"

# Regular expression to extract code2, code3, and name
pattern = re.compile(r'code2:\s*"(\w+)",\s*code3:\s*"(\w+)",\s*name:\s*"([^"]+)"')

# SQL template for inserting into the public."Countries" table
sql_template = 'INSERT INTO public."Countries" ("Code3", "Code2", "Name", "Visited") VALUES (\'{code3}\', \'{code2}\', \'{name}\', false);'

# Process the input file and write to the output file
with open(input_file, "r") as infile, open(output_file, "w") as outfile:
    for line in infile:
        match = pattern.search(line)
        if match:
            code2, code3, name = match.groups()
            # Escape single quotes in the name to avoid SQL errors
            name_escaped = name.replace("'", "''")
            sql_statement = sql_template.format(code2=code2, code3=code3, name=name_escaped)
            outfile.write(sql_statement + "\n")

print(f"SQL file '{output_file}' has been created.")
