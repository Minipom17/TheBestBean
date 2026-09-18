import sqlite3
import datetime

conn = sqlite3.connect('coffee.db')
c = conn.cursor()

drafts = [
    ('The Complete Guide to Cusco Coffee Culture', 'cusco-coffee-culture-guide', 'Drafting content about specialty coffee, local cafes, and the history of coffee in the Cusco region. Everything you need to know about navigating the booming specialty coffee scene in Cusco.'),
    ("La Convencion: Peru's Hidden Coffee Region", 'la-convencion-peru-coffee', "Drafting content about Santa Teresa, high altitude farming, and the unique flavor profiles of La Convencion. Discover the high-altitude farms and unique flavors of La Convencion, Cusco's premier coffee growing region."),
    ('SCA Scoring Explained: What Makes Coffee Specialty?', 'sca-scoring-explained-specialty-coffee', 'Drafting content explaining the 100-point SCA scale, defects, and what 80+ actually means in the cup. Ever wondered what makes a coffee specialty? We break down the SCA scoring system and how it impacts your daily brew.')
]

now = datetime.datetime.now().isoformat()
for title, slug, content in drafts:
    c.execute('''
        INSERT INTO BlogPosts (Title, TitleES, Slug, Content, ContentES, ImageUrl, CreatedAt, Author, IsPublished, PublishedAt)
        VALUES (?, '', ?, ?, '', '', ?, 'Purple Bean Team', 0, NULL)
    ''', (title, slug, content, now))

conn.commit()
conn.close()
print('Drafts inserted.')
