# TODO: if python3.10, import yaml won't work
import yaml

def read_service_file(config_path):
    with open(config_path, "r") as f:
        services = f.read() 
        services = yaml.load(services, Loader=yaml.Loader)
    
    return services["services"]


