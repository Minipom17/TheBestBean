import vtracer
import os

input_path = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\.user_uploaded\media_1789957535127.png'
output_path = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\farmer_vector.svg'

vtracer.convert_image_to_svg_py(
    input_path,
    output_path,
    colormode='binary',    
    hierarchical='stacked', 
    mode='spline',         
    filter_speckle=4,      
    color_precision=6,
    layer_difference=16,
    corner_threshold=60,
    length_threshold=4.0,
    max_iterations=10,
    splice_threshold=45,
    path_precision=3
)
print("Vectorized to", output_path)
