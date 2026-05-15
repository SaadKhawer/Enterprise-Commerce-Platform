with open("c:/Users/saads/Desktop/ShopWaveBlazor/Components/Pages/Settings.razor.css", "r") as f:
    css_content = f.read()

# Escape @media for Razor compiler
css_content = css_content.replace("@media", "@@media")

with open("c:/Users/saads/Desktop/ShopWaveBlazor/Components/Pages/Settings.razor", "r") as f:
    razor_content = f.read()

# Append CSS wrapped in <style> block
with open("c:/Users/saads/Desktop/ShopWaveBlazor/Components/Pages/Settings.razor", "w") as f:
    f.write(razor_content + "\n<style>\n" + css_content + "\n</style>")
