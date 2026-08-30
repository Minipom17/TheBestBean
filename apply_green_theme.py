import os

pages_dir = "Pages"
for root, _, files in os.walk(pages_dir):
    for file in files:
        if file.endswith(".cshtml"):
            file_path = os.path.join(root, file)
            with open(file_path, "r", encoding="utf-8") as f:
                content = f.read()
            
            # Replace dark mode colors with Olive Green Theme
            content = content.replace("dark:bg-[#111111]", "dark:bg-[#3C4A33]")
            content = content.replace("dark:bg-[#1A1A1A]", "dark:bg-[#45553B]")
            content = content.replace("dark:text-[#F5F5F5]", "dark:text-[#F5F5F7]")
            content = content.replace("dark:text-white", "dark:text-[#F5F5F7]")
            
            content = content.replace("dark:border-[#F5F5F5]/10", "dark:border-[#4F6044]")
            content = content.replace("dark:border-[#F5F5F5]/20", "dark:border-[#4F6044]")
            content = content.replace("dark:border-[#F5F5F5]", "dark:border-[#4F6044]")
            
            with open(file_path, "w", encoding="utf-8") as f:
                f.write(content)

print("Applied Olive Green Theme to Dark Mode.")
