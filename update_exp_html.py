import sys
import re

file_path = r'c:\Users\alext\source\repos\TheBestBean\TheBestBean\Pages\Experiences.cshtml'

with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

pattern = r'<h3([^>]*)>@exp\.Title</h3>'
def repl(m):
    return f'''<h3{m.group(1)}>
                                        @{{
                                            var titleParts = exp.Title.Split('[');
                                            if (titleParts.Length > 1)
                                            {{
                                                <text>@titleParts[0].Trim() <span class="text-[0.55em] text-v-gray font-normal tracking-normal ml-0 md:ml-2 block sm:inline">[@titleParts[1]</span></text>
                                            }}
                                            else
                                            {{
                                                @exp.Title
                                            }}
                                        }}
                                    </h3>'''

new_content, count = re.subn(pattern, repl, content)
if count > 0:
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(new_content)
    print(f'Replaced {count} instances via regex successfully.')
else:
    print('Regex failed to find @exp.Title</h3>')
