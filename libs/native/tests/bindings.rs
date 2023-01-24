use interoptopus::util::NamespaceMappings;
use interoptopus::Error;
use interoptopus::Interop;
use swc_dotnet::my_inventory;

#[test]
#[cfg_attr(miri, ignore)]
fn bindings_csharp() -> Result<(), Error> {
    use interoptopus_backend_csharp::{Config, Generator};

    Generator::new(
        Config {
            class: "SwcWrapInterop".to_string(),
            dll_name: "swc_dotnet".to_string(),
            namespace_mappings: NamespaceMappings::new("SwcDotNet"),
            ..Config::default()
        },
        my_inventory(),
    )
        .write_file("../SwcDotNet/SwcWrapInterop.cs")?;

    Ok(())
}
