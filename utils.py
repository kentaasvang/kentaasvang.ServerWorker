try:
    import yaml
except ImportError:
    print("You need to run 'pip3 install pyyaml'")
    exit(0)

def read_service_file(config_path):
    with open(config_path, "r") as f:
        services = f.read() 
        services = yaml.load(services, Loader=yaml.Loader)
    
    return services["services"]


